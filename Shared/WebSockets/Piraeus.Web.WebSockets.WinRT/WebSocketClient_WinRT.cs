using Piraeus.Web.WebSockets;
using Piraeus.Web.WebSockets.WinRT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Web;

[assembly: Xamarin.Forms.Dependency(typeof(WebSocketClient_WinRT))]
namespace Piraeus.Web.WebSockets.WinRT
{
    //[assembly: Xamarin.Forms.Dependency(typeof(WebSocketClient_WinRT))]
    public class WebSocketClient_WinRT : IWebSocketClient
    {
        private const int receiveChunkSize = 1024;
        private MessageWebSocket client;
        private bool connected = false;

        public event WebSocketEventHandler OnOpen;
        public event WebSocketEventHandler OnClose;
        public event WebSocketErrorHandler OnError;
        public event WebSocketMessageHandler OnMessage;

        public WebSocketClient_WinRT()
        {
            this.client = new MessageWebSocket();
        }

        public Task ConnectAsync(string host)
        {
            return ConnectAsync(host, null, null);
        }

        public async Task ConnectAsync(string host, string subprotocol, string securityToken)
        {
            //client.Options.SetBuffer(1024, 1024);
            this.client.Control.OutboundBufferSizeInBytes = 1024;

            if (!string.IsNullOrEmpty(subprotocol))
            {
                //this.client.Options.AddSubProtocol(subprotocol);
                this.client.Control.SupportedProtocols.Add(subprotocol);
            }

            if (!string.IsNullOrEmpty(securityToken))
            {
                this.client.SetRequestHeader("Authorization", String.Format("Bearer {0}", securityToken));
            }

            this.client.MessageReceived += Client_MessageReceived;

            try
            {
                await client.ConnectAsync(new Uri(host));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Web socket client failed to connect.");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }

            connected = true;

            if (OnOpen != null)
            {
                OnOpen(this, "Web socket is opened.");
            }
        }

        private void Client_MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            try
            {
                var messageReader = args.GetDataReader();
                int length = messageReader.ReadInt32();
                byte[] message = new byte[length];
                messageReader.ReadBytes(message);

                if (OnMessage != null)
                {
                    OnMessage(this, message);
                }
            }
            catch (Exception ex)
            {
                WebErrorStatus status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
                //Trace.TraceWarning("Web Socket exception during send.");
                System.Diagnostics.Debug.WriteLine(ex.Message);

                client.Dispose();
                client = null;
                connected = false;
            }
        }

        public void Close()
        {
            // 1000: The purpose of the connection has been fulfilled and the connection is no longer needed.
            this.client.Close(1000, "Web Socket closed by client.");

            if (OnClose != null)
            {
                OnClose(this, "Web socket is closed.");
            }
        }

        public async Task SendAsync(byte[] message)
        {
            try
            {
                using (var messageWriter = new DataWriter(this.client.OutputStream))
                {
                    messageWriter.WriteInt32(message.Length);
                    messageWriter.WriteBytes(message);
                    await messageWriter.StoreAsync();
                    messageWriter.DetachStream();
                }
            }
            catch (Exception ex)
            {
                WebErrorStatus status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
                //Trace.TraceWarning("Web Socket exception during send.");
                System.Diagnostics.Debug.WriteLine(ex.Message);

                client.Dispose();
                client = null;
                connected = false;
            }
        }
    }
}
