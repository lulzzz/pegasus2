using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Piraeus.Web.WebSockets;
using Piraeus.Web.WebSockets.Net45;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;

[assembly: Xamarin.Forms.Dependency(typeof(WebSocketClient_Net45))]
namespace Piraeus.Web.WebSockets.Net45
{

    //[assembly: Xamarin.Forms.Dependency(typeof(WebSocketClient_Net45))]
    public class WebSocketClient_Net45 : IWebSocketClient
    {
        private const int receiveChunkSize = 1024;
        private ClientWebSocket client;

        public event WebSocketEventHandler OnOpen;
        public event WebSocketEventHandler OnClose;
        public event WebSocketErrorHandler OnError;
        public event WebSocketMessageHandler OnMessage;

        private Queue<byte[]> messageQueue;
        public WebSocketClient_Net45()
        {
            this.client = new ClientWebSocket();
            this.messageQueue = new Queue<byte[]>();
        }


        public bool IsConnected
        {
            get
            {
                if(this.client == null)
                {
                    return false;
                }
                else
                {
                    return this.client.State == WebSocketState.Open;
                }
            }
        }

        public async Task ConnectAsync(string host)
        {
            await ConnectAsync(host, null, null);
        }

        public async Task ConnectAsync(string host, string subprotocol, string securityToken)
        {
            client.Options.SetBuffer(1024, 1024);
            
            if(!string.IsNullOrEmpty(subprotocol))
            {
                this.client.Options.AddSubProtocol(subprotocol);
            }

            if (!string.IsNullOrEmpty(securityToken))
            {
                client.Options.SetRequestHeader("Authorization", String.Format("Bearer {0}", securityToken));
            }
            try
            {
                await client.ConnectAsync(new Uri(host), CancellationToken.None);
            }
            catch(Exception ex)
            {
                //Trace.TraceWarning("Web socket client failed to connect.");
                //Trace.TraceError(ex.Message);
                throw;
            }

            ReceiveAsync();

            if (OnOpen != null)
            {
                OnOpen(this, "Web socket is opened.");
            }                      
        }

        public async Task CloseAsync()
        {
            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Web Socket closed by client.", CancellationToken.None);

            if(OnClose != null)
            {
                OnClose(this, "Web socket is closed.");
            }
        }

        public async Task ReceiveAsync()
        {
            Exception exception = null;
            byte[] prefix = null;
            int offset = 0;
            WebSocketReceiveResult result = null;
            int remainingLength = 0;

            while(client.State == WebSocketState.Open)
            {
                try
                {
                    if (prefix == null)
                    {
                        prefix = new byte[4];
                        result = await client.ReceiveAsync(new ArraySegment<byte>(prefix), CancellationToken.None);
                        prefix = BitConverter.IsLittleEndian ? prefix.Reverse().ToArray() : prefix;
                        remainingLength = BitConverter.ToInt32(prefix, 0);
                    }
                    else
                    {
                        int index = 0;
                        byte[] message = new byte[remainingLength];
                        do
                        {
                            int bufferSize = remainingLength > receiveChunkSize ? receiveChunkSize : remainingLength;
                            byte[] buffer = new byte[bufferSize];
                            result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                            
                            Buffer.BlockCopy(buffer, 0, message, index, buffer.Length);
                            index += bufferSize;
                            remainingLength = buffer.Length - index;
                        } while (remainingLength > 0);


                        prefix = null;

                        if(!result.EndOfMessage)
                        {
                            throw new WebSocketException("Expected EOF for Web Socket message received.");
                        }

                        if(OnMessage != null)
                        {
                            OnMessage(this, message);
                        }
                    }
                }
                catch(Exception ex)
                {
                    exception = ex;
                    //Trace.TraceWarning("Web socket receive faulted.");
                    //Trace.TraceError(ex.Message);
                }
            }

            if (exception != null)
            {
                await CloseAsync();

                if (OnClose != null)
                {
                    OnClose(this, "Client forced to close.");
                }
            }
        }

        public async Task SendAsync(byte[] message)
        {
            Exception exception = null;
            try
            {
                this.messageQueue.Enqueue(message);

                while (this.messageQueue.Count > 0)
                {
                    byte[] prefix = BitConverter.IsLittleEndian ? BitConverter.GetBytes(message.Length).Reverse().ToArray() : BitConverter.GetBytes(message.Length);
                    byte[] messageBuffer = new byte[message.Length + prefix.Length];
                    Buffer.BlockCopy(prefix, 0, messageBuffer, 0, prefix.Length);
                    Buffer.BlockCopy(message, 0, messageBuffer, prefix.Length, message.Length);

                    int remainingLength = messageBuffer.Length;
                    int index = 0;
                    do
                    {
                        int bufferSize = remainingLength > receiveChunkSize ? receiveChunkSize : remainingLength;
                        byte[] buffer = new byte[bufferSize];
                        Buffer.BlockCopy(messageBuffer, index, buffer, 0, bufferSize);
                        index += bufferSize;
                        remainingLength = messageBuffer.Length - index;
                        try
                        {
                            await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, remainingLength == 0, CancellationToken.None);
                        }
                        catch (Exception ex)
                        {
                            //Trace.TraceWarning("Web Socket send fault.");
                            //Trace.TraceError(ex.Message);
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
                exception = ex;
                //Trace.TraceWarning("Web Socket exception during send.");
                //Trace.TraceError(ex.Message);                
            }

            if (exception != null)
            {
                await CloseAsync();

                if (OnClose != null)
                {
                    OnClose(this, "Client forced to close.");
                }
            }
        }
    }
}