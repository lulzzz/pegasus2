using Newtonsoft.Json;
using Piraeus.Protocols.Coap;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Piraeus.Web.WebSockets;
using NAE.Data;

namespace NAE.FieldGateway.Channels
{

    //public delegate void MpdEventHandler(object sender, ParachuteCommand message);
    //public delegate void MpdTimerEventHandler(object sender, MpdTime message);
    //public delegate void MpdArmEventHandler(object sender, MpdArm message);
    //public delegate void DsrEventHandler(object sender, DeliverySystemCommand message);
    public delegate void UserNoteEventHandler(object sender, UserMessage message);
    //public delegate void CameraEventHandler(object sender, CameraCommand message);
    public delegate void WebSocketStatusEventHandler(object sender, string message);

    public class WebSocketManager
    {

        public WebSocketManager(string host, string subprotocol, string token, bool autoReset, string[] autoSubscribeTopics, string authority = "pegasusmission.io")
        {
            this.host = host;
            this.subprotocol = subprotocol;
            this.token = token;
            this.autoReset = autoReset;

            subscriptions = new List<Uri>();
            int index = 0;

            while(index < autoSubscribeTopics.Length)
            {
                subscriptions.Add(new Uri(String.Format("coaps://{0}/subscribe?topic={1}", authority, autoSubscribeTopics[index])));
                index++;
            }

            //subscriptions.Add(new Uri(String.Format("coaps://{0}/subscribe?topic={1}", CoapAuthority, DsrCommandTopic)));
            //subscriptions.Add(new Uri(String.Format("coaps://{0}/subscribe?topic={1}", CoapAuthority, MpdCommandTopic)));
            //subscriptions.Add(new Uri(String.Format("coaps://{0}/subscribe?topic={1}", CoapAuthority, VideoCommandTopic)));
            //subscriptions.Add(new Uri(String.Format("coaps://{0}/subscribe?topic={1}", CoapAuthority, MpdArmedTopic)));
            //subscriptions.Add(new Uri(String.Format("coaps://{0}/subscribe?topic={1}", CoapAuthority, MpdTimerTopic)));
            //subscriptions.Add(new Uri(String.Format("coaps://{0}/subscribe?topic={1}", CoapAuthority, UserNoteTopic)));
        }

        //public event MpdEventHandler OnMpd;
        //public event MpdArmEventHandler OnMpdArm;
        //public event MpdTimerEventHandler OnMpdTimer;
        //public event DsrEventHandler OnDsr;
        public event UserNoteEventHandler OnUserNote;
        //public event CameraEventHandler OnCamera;
        public event WebSocketStatusEventHandler OnStatusChange;

        //private const string UserNoteTopic = "http://pegasus2.org/usernote";
        //private const string DsrCommandTopic = "http://pegasus2.org/release"; 
        //private const string MpdCommandTopic = "http://pegasus2.org/deploy";
        //private const string VideoCommandTopic = "http://pegasus2.org/video";
        //private const string MpdArmedTopic = "http://pegasus2.org/mpdarmed";
        //private const string MpdTimerTopic = "http://pegasus2.org/mpdtimer";
        private const string CoapAuthority = "pegasusmission.io";
        

        private ushort messageId;

        private List<Uri> subscriptions;

        private string host;
        private string subprotocol;
        private string token;
        private bool autoReset;
        private WebSocketClient client;
        private const string authority = "http://pegasus2.org/";


        public bool IsConnected
        {
            get
            {
                if(client != null)
                {
                    return client.IsConnected;
                }
                else
                {
                    return false;
                }
            }
        }

        public void Close()
        {
            if (client != null)
            {
                if (client.IsConnected)
                {
                    Task task = Task.Factory.StartNew(async () =>
                    {
                        await client.CloseAsync();
                    });

                    Task.WhenAll(task);
                }
            }
        }

        public void Connect()
        {
            if (client != null && client.IsConnected)
            {
                return;
            }

            if(client == null)
            {
                client = new WebSocketClient();
            }

            if(client.IsConnected)
            {
                Task closeTask = client.CloseAsync();
                Task.WaitAny(closeTask);

                if(OnStatusChange != null)
                {
                    OnStatusChange(this, "Closed");
                }

                client = null;
            }

            if (OnStatusChange != null)
            {
                OnStatusChange(this, "Connecting");
            }

            client = new WebSocketClient();

            client.OnClose += client_OnClose;
            client.OnError += client_OnError;
            client.OnOpen += client_OnOpen;
            client.OnMessage += client_OnMessage;

            Task task = Task.Factory.StartNew(async () =>
            {
                await client.ConnectAsync(host, subprotocol, token);
            });

            Task.WhenAll(task);
            

            if(OnStatusChange != null)
            {
                OnStatusChange(this, "Connected");
            }

            foreach(Uri uri in subscriptions)
            {
                CoapRequest request = new CoapRequest(messageId++, RequestMessageType.NonConfirmable, MethodType.POST, uri, MediaType.Json);
                Task subTask = client.SendAsync(request.Encode());
                Task.WaitAny(subTask);
            }
            
        }
        public void ResetConnection()
        {
            if(OnStatusChange != null)
            {
                OnStatusChange(this,"Reset");
            }

            if (client != null)
            {
                client.OnClose -= client_OnClose;
                client.OnError -= client_OnError;
                client.OnOpen -= client_OnOpen;
                client.OnMessage -= client_OnMessage;
            }

            client = null;

            Thread.Sleep(10000);
            Connect();
        }

        public async Task SubscribeAsync(Uri uri)
        {
            CoapRequest request = new CoapRequest(messageId++, RequestMessageType.NonConfirmable, MethodType.POST, uri, MediaType.Json);
            byte[] message = request.Encode();
            await client.SendAsync(message);
        }

        public void Subscribe(Uri uri)
        {
            CoapRequest request = new CoapRequest(messageId++, RequestMessageType.NonConfirmable, MethodType.POST, uri, MediaType.Json);
            byte[] message = request.Encode();
            Task task = client.SendAsync(message);
            Task.WaitAny(task);

            Trace.TraceInformation("Subscribed - {0}", uri.ToString());
        }

        public void SendMessage(NAE.Data.Telemetry message)
        {
            string jsonString = JsonConvert.SerializeObject(message);
            byte[] payload = Encoding.UTF8.GetBytes(jsonString);

            if(messageId == ushort.MaxValue || messageId == 0)
            {
                messageId = 1;
            }

            string resource = "http://pegasusnae.org/telemetry";
            Uri requestUri = new Uri(String.Format("coaps://{0}/publish?topic={1}", CoapAuthority, resource));
            CoapRequest request = new CoapRequest(messageId++, RequestMessageType.NonConfirmable, MethodType.POST, requestUri, MediaType.Json, payload);
            byte[] coap = request.Encode();

            try
            {
                Task task = client.SendAsync(coap);
                Task.WhenAll(task);
                Trace.TraceInformation("Message sent to cloud.");
                Trace.TraceInformation(jsonString);
            }
            catch(Exception ex)
            {
                Trace.TraceWarning("Web Socket expection.");
                Trace.TraceError(ex.Message);
            }
        }

       

        #region Web Socket events
        void client_OnMessage(object sender, byte[] message)
        {
            
            FileSystemWriter writer = FileSystemWriter.Create();

            Task task = null;
            CoapMessage coapMessage = CoapMessage.DecodeMessage(message);
            string jsonString = Encoding.UTF8.GetString(coapMessage.Payload);
            string part1 = coapMessage.ResourceUri.ToString().Remove(0, coapMessage.ResourceUri.ToString().IndexOf("?"));

            if(part1.Contains("usernote"))
            {
                task = writer.WriteAsync("User_Message", jsonString);
                Task.WaitAny(task);

                UserMessage userMessage = UserMessage.Load(jsonString);

                if (OnUserNote != null)
                {
                    OnUserNote(this, userMessage);
                }
            }
        }

        void client_OnOpen(object sender, string message)
        {
            if(OnStatusChange != null)
            {
                OnStatusChange(this, "Connected");
            }
        }

        void client_OnError(object sender, Exception ex)
        {
            Trace.TraceWarning("Web Socket error.");
            Trace.TraceError(ex.Message);

            if(OnStatusChange != null)
            {
                OnStatusChange(this, "Error");
            }
        }

        void client_OnClose(object sender, string message)
        {
            if(OnStatusChange != null)
            {
                OnStatusChange(this, "Closed");
            }

            if(autoReset)
            {
                ResetConnection();
            }
        }

        #endregion

    }
}
