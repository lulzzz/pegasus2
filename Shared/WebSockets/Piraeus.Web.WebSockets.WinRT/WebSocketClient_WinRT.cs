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

        private Queue<byte[]> messageQueue;

        public WebSocketClient_WinRT()
        {
            this.client = new MessageWebSocket();
            this.messageQueue = new Queue<byte[]>();
        }
        
        public async Task ConnectAsync(string host)
        {
            await ConnectAsync(host, null, null);
        }

        public async Task ConnectAsync(string host, string subprotocol, string securityToken)
        {
            //client.Options.SetBuffer(1024, 1024);
            this.client.Control.OutboundBufferSizeInBytes = 1024;
            
            if(!string.IsNullOrEmpty(subprotocol))
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
            catch(Exception ex)
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
            if (args.MessageType != SocketMessageType.Binary)
            {
                throw new InvalidOperationException("Expected a Binary message!");
            }

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
            catch(Exception ex)
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

            if(OnClose != null)
            {
                OnClose(this, "Web socket is closed.");
            }
        }

        public async Task SendAsync(byte[] message)
        {
            try
            {
                var messageWriter = new DataWriter(this.client.OutputStream);
                this.messageQueue.Enqueue(message);

                while (this.messageQueue.Count > 0)
                {
                    byte[] prefix = BitConverter.IsLittleEndian ? BitConverter.GetBytes(message.Length).Reverse().ToArray() : BitConverter.GetBytes(message.Length);
                    byte[] messageBuffer = new byte[message.Length + prefix.Length];
                    System.Buffer.BlockCopy(prefix, 0, messageBuffer, 0, prefix.Length);
                    System.Buffer.BlockCopy(message, 0, messageBuffer, prefix.Length, message.Length);

                    int remainingLength = messageBuffer.Length;
                    int index = 0;
                    do
                    {
                        int bufferSize = remainingLength > receiveChunkSize ? receiveChunkSize : remainingLength;
                        byte[] buffer = new byte[bufferSize];
                        System.Buffer.BlockCopy(messageBuffer, index, buffer, 0, bufferSize);
                        index += bufferSize;
                        remainingLength = messageBuffer.Length - index;
                        try
                        {
                            messageWriter.WriteString(buffer.ToString());
                            await messageWriter.StoreAsync();
                        }
                        catch (Exception ex)
                        {
                            //Trace.TraceWarning("Web Socket send fault.");
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                            throw;

                        }
                        finally
                        {
                            this.messageQueue.Dequeue();
                        }
                    } while (remainingLength > 0);
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
