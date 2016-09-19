using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Piraeus.Protocols.Coap;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Piraeus.Web.WebSockets;
using Newtonsoft.Json;
using PegasusMissionWeb.Security;
using System.Security.Claims;
using System.Diagnostics;
using System.Web.Script.Serialization;
using NAE.Data;
using System.Net;

namespace PegasusMissionWeb.Hubs
{
    public class PegasusHub : Hub
    {
        string host = "ws://broker.pegasusmission.io/api/connect";
        string subprotocol = "coap.v1";
        private string telemetryUriString = "coaps://pegasusmission.io/subscribe?topic=http://pegasusnae.org/telemetry";      
        private string issuer = "urn:pegasusmission.io";
        private string audience = "http://broker.pegasusmission.io/api/connect";
        private string signingKey = "cW0iA3P/mhFi0/O4EAja7UuJ16q6Aeg4cOzL7SIvLL8=";

        public bool webSocketInstantiated = false;

        private bool opened;

        public override Task OnDisconnected(bool stopCalled)
        {
            ClientManager.Disconnect(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
        
        public override Task OnConnected()
        {            
            if (webSocketInstantiated == false)
            {
                WebSocketClient client = new WebSocketClient();

                Task task = Task.Factory.StartNew(async () =>
                {
                    
                    webSocketInstantiated = true;
                    client.OnError += client_OnError;
                    client.OnOpen += client_OnOpen;
                    client.OnClose += client_OnClose;
                    client.OnMessage += client_OnMessage;

                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim("http://pegasusmission.io/claims/name", Guid.NewGuid().ToString()));
                    claims.Add(new Claim("http://pegasusmission.io/claims/role", "user"));

                    string tokenString = JwtSecurityTokenBuilder.Create(issuer, audience, claims, 20000, signingKey);
                    
                    await client.ConnectAsync(host, subprotocol, tokenString);
                });

                Task.WhenAll(task);

                while (!opened)
                {
                    Thread.Sleep(100);
                }

                ClientManager.Add(Context.ConnectionId, client);
                Subscribe();
            }

            return base.OnConnected();
        }
     

        public void Send(string jsonString)
        {            
            Clients.All.addNewMessageToPage(jsonString);
        }
        public void Onboard(string runId)
        {            
            Clients.All.updateVideos(parseJsonForVideos(runId));
            parseConfJson(runId);
        }
        
        public void UpdateOnboard(string jsonString)
        {
            Clients.All.UpdateOnboard(jsonString);
        }

        public void Subscribe()
        {
            WebSocketClient client = ClientManager.Get(Context.ConnectionId);
            Random ran = new Random();
            ushort id = (ushort)ran.Next(1, Convert.ToInt32(ushort.MaxValue - 10));
            CoapRequest telemetrySubscription = new CoapRequest(id++, RequestMessageType.NonConfirmable, MethodType.POST, new Uri(telemetryUriString), MediaType.Json);

            byte[] telemetrySubscribe = telemetrySubscription.Encode();
            client.Send(telemetrySubscribe);     
        }
        private string parseJsonForVideos(string runId)
        {
            var configUrl = System.Configuration.ConfigurationManager.AppSettings["configUrl"];
            var listToReturn = new List<string>();
            using (var client = new WebClient())
            {
                var configFile = JsonConvert.DeserializeObject<List<Config>>(client.DownloadString(new Uri(configUrl)));
                var neededConfigFile = configFile.Where(n => n.RunId.Equals(runId)).First();
                var runToShow = neededConfigFile.OnboardTelemetryUrl;
                
                listToReturn.Add(JsonConvert.SerializeObject(neededConfigFile.Drone1VideoUrl));
                listToReturn.Add(JsonConvert.SerializeObject(neededConfigFile.Drone2VideoUrl));
                listToReturn.Add(JsonConvert.SerializeObject(neededConfigFile.OnboardVideoUrl));
                string[][] newKeys = listToReturn.Select(x => new string[] { x }).ToArray();

                
                return JsonConvert.SerializeObject(newKeys);
            }
            return "0";
        }
        private void parseConfJson(string runId)
        {
            var configUrl = System.Configuration.ConfigurationManager.AppSettings["configUrl"];
            var listToReturn = new List<string>();
            using (var client = new WebClient())
            {
                var configFile = JsonConvert.DeserializeObject<List<Config>>(client.DownloadString(new Uri(configUrl)));
                var neededConfigFile = configFile.Where(n => n.RunId.Equals(runId)).First();
                var runToShow = neededConfigFile.OnboardTelemetryUrl;
                
               
                var telemetryFile = JsonConvert.DeserializeObject<List<EagleTelemetry>>(client.DownloadString(new Uri(runToShow)));
                PegasusHub hub = new PegasusHub();
                foreach (EagleTelemetry t in telemetryFile)
                {
                    Thread.Sleep(500);
                    UpdateOnboard(JsonConvert.SerializeObject(t));
                }
            }
        }
        private async void parseConfJson()
        {
            var configUrl = System.Configuration.ConfigurationManager.AppSettings["configUrl"];

            using (var client = new WebClient())
            {
                var configFile = JsonConvert.DeserializeObject<List<Config>>(client.DownloadString(new Uri(configUrl)));
                foreach (Config n in configFile)
                {

                    var telemetryFile = JsonConvert.DeserializeObject<List<EagleTelemetry>>(client.DownloadString(new Uri(n.OnboardTelemetryUrl)));
                    foreach(EagleTelemetry t in telemetryFile)
                    {                        
                        Send(JsonConvert.SerializeObject(t));
                    }
                    
                }
            }
        }

        #region events handlers
        public void client_OnMessage(object sender, byte[] message)
        {
            CoapMessage coapMessage = CoapMessage.DecodeMessage(message);
            //Trace.TraceInformation(coapMessage.ResourceUri.ToString());
            string jsonString = Encoding.UTF8.GetString(coapMessage.Payload);           

            Send(jsonString);
            parseConfJson();
        }

        void client_OnClose(object sender, string message)
        {
            opened = false;
            webSocketInstantiated = false;
        }

        void client_OnOpen(object sender, string message)
        {
            opened = true;
        }

        void client_OnError(object sender, Exception ex)
        {
            opened = false;
            
        }

        #endregion
    }
}