using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Piraeus.Web.WebSockets;
using Piraeus.Web.WebSockets.Net45;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

[assembly: Xamarin.Forms.Dependency(typeof(WebSocketClient_Net45))]
namespace Piraeus.Web.WebSockets.Net45
{

    //[assembly: Xamarin.Forms.Dependency(typeof(WebSocketClient_Net45))]
    public class WebSocketClient_Net45 : IWebSocketClient
    {
        private const int receiveChunkSize = 256;
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

        public Task ConnectAsync(string host)
        {
            return ConnectAsync(host, null, null);
        }

        public async Task ConnectAsync(string host, string subprotocol, string securityToken)
        {
            client.Options.SetBuffer(1024, 1024);
            
            if (!string.IsNullOrEmpty(subprotocol))
            {
                this.client.Options.AddSubProtocol(subprotocol);
            }

            if (!string.IsNullOrEmpty(securityToken))
            {
                client.Options.SetRequestHeader("Authorization", String.Format("Bearer {0}", securityToken));
            }

            await client.ConnectAsync(new Uri(host), CancellationToken.None);

            Thread receiveLoopThread = new Thread(ReceiveLoopAsync);
            receiveLoopThread.Start();

            if (OnOpen != null)
            {
                OnOpen(this, "Web socket is opened.");
            }                      
        }

        public async Task CloseAsync()
        {
            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Web Socket closed by client.", CancellationToken.None);
            // TODO: reap thread

            if (OnClose != null)
            {
                OnClose(this, "Web socket is closed.");
            }
        }

        private async void ReceiveLoopAsync()
        {
            Exception exception = null;
            WebSocketReceiveResult result = null;

            while (client.State == WebSocketState.Open && exception == null)
            {
                int remainingLength = 0;

                try
                {
                    byte[] prefix = new byte[4];
                    //result = await client.ReceiveAsync(new ArraySegment<byte>(prefix), CancellationToken.None);                                                
                    //prefix = BitConverter.IsLittleEndian ? prefix.Reverse().ToArray() : prefix;
                    //remainingLength = BitConverter.ToInt32(prefix, 0);  
                    int prefixSize = 4;
                    int offset = 0;
                    while (offset < prefix.Length)
                    {
                        byte[] array = new byte[prefixSize - offset];
                        result = await client.ReceiveAsync(new ArraySegment<byte>(array), CancellationToken.None);
                        Buffer.BlockCopy(array, 0, prefix, offset, result.Count);
                        offset += result.Count;
                        if (prefix.Length < 4)
                        {
                            Trace.TraceInformation("Prefix too short.");
                        }
                    }

                    prefix = BitConverter.IsLittleEndian ? prefix.Reverse().ToArray() : prefix;
                    remainingLength = BitConverter.ToInt32(prefix, 0);

                    int index = 0;
                    byte[] message = new byte[remainingLength];
//                    do
//                    {
                        int bufferSize = remainingLength > receiveChunkSize ? receiveChunkSize : remainingLength;
                        byte[] buffer = new byte[bufferSize];

                        Label_1E9:
                        result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        Trace.WriteLine("Received " + result.Count + " bytes, remainingLength: " + remainingLength);
                        if (result.Count < remainingLength)
                        {
                            Buffer.BlockCopy(buffer, 0, message, index, result.Count);
                            remainingLength = remainingLength - result.Count;
                            index += result.Count;
                            goto Label_1E9;
                        }
                        else
                        {
                            Buffer.BlockCopy(buffer, 0, message, index, result.Count);
                            //Buffer.BlockCopy(buffer, 0, message, index, buffer.Length);
                            //index += bufferSize;
                            //remainingLength = buffer.Length - index;
                            remainingLength = remainingLength - result.Count;
                        }

//                    } while (remainingLength > 0);

                    if (!result.EndOfMessage)
                    {
                        if (OnError != null)
                        {
                            OnError(this, new WebSocketException("Expected EOF for Web Socket message received."));
                        }
                        else
                        {
                            throw new WebSocketException("Expected EOF for Web Socket message received.");
                        }
                    }

                    if (OnMessage != null)
                    {
                        OnMessage(this, message);
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                    Trace.TraceWarning("Web socket receive faulted.");
                    Trace.TraceError(ex.Message);
                    break;
                }
            }

            if (exception != null)
            {
                if (OnError != null)
                {
                    OnError(this, new WebSocketException(exception.Message));
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
                        await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, remainingLength == 0, CancellationToken.None);
                        this.messageQueue.Dequeue();
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
                if (OnError != null)
                {
                    OnError(this, exception);
                }
            }
        }
    }
}