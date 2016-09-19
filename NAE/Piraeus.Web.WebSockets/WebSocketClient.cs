using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Piraeus.Web.WebSockets
{
    public delegate void WebSocketEventHandler(object sender, string message);
    public delegate void WebSocketErrorHandler(object sender, Exception ex);
    public delegate void WebSocketMessageHandler(object sender, byte[] message);
    public class WebSocketClient
    {
        public WebSocketClient()
        {
            this.client = new ClientWebSocket();
            this.messageQueue = new Queue<byte[]>();
        }

        private System.Timers.Timer timer;

        public bool IsConnected
        {
            get
            {
                if (this.client == null)
                {
                    return false;
                }
                else
                {
                    return this.client.State == WebSocketState.Open;
                }
            }
        }

        private const int receiveChunkSize = 1024;
        private ClientWebSocket client;
        public event WebSocketEventHandler OnOpen;
        public event WebSocketEventHandler OnClose;
        public event WebSocketErrorHandler OnError;
        public event WebSocketMessageHandler OnMessage;
        private Queue<byte[]> messageQueue;
        

        public async Task ConnectAsync(string host)
        {
            await ConnectAsync(host, null, null);
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
            try
            {
                await client.ConnectAsync(new Uri(host), CancellationToken.None);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Web socket client failed to connect.");
                Trace.TraceError(ex.Message);

                if (OnError != null)
                {
                    OnError(this, new WebSocketException("Web Socket failed to connect."));
                }

                throw;
            }

            if (OnOpen != null)
            {
                OnOpen(this, "Web socket is opened.");
                timer = new System.Timers.Timer(2000);
                timer.Elapsed += timer_Elapsed;
                timer.Start();

            }

            await ReceiveAsync();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!IsConnected)
            {
                if (OnClose != null)
                {
                    OnClose(this, "Web socket is closed.");
                }

                timer.Stop();
            }


        }

        public async Task CloseAsync()
        {
            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Web Socket closed by client.", CancellationToken.None);

            if (OnClose != null)
            {
                OnClose(this, "Web socket is closed.");
            }
        }

        public async Task ReceiveAsync()
        {
            Exception exception = null;
            byte[] prefix = null;
            WebSocketReceiveResult result = null;
            int remainingLength = 0;

            while (client.State == WebSocketState.Open)
            {
                try
                {
                    if (prefix == null)
                    {
                        prefix = new byte[4];
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

                    }
                    else
                    {
                        int index = 0;
                        byte[] message = new byte[remainingLength];
                        do
                        {
                            int bufferSize = remainingLength > receiveChunkSize ? receiveChunkSize : remainingLength;
                            byte[] buffer = new byte[bufferSize];
                            Label_1E9:
                            result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
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
                                remainingLength = remainingLength - result.Count;
                            }

                        } while (remainingLength > 0);

                        prefix = null;

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
                }
                catch (Exception ex)
                {
                    exception = ex;
                    //Console.WriteLine("Web Socket Exception");
                    //Console.WriteLine(ex.Message);
                    //Console.WriteLine(ex.StackTrace);

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

                await CloseAsync();

                if (OnClose != null)
                {
                    OnClose(this, "Client forced to close.");
                }
            }
        }

        public void Send(byte[] messageBytes)
        {
            this.messageQueue.Enqueue(messageBytes);
            Exception exception = null;
            try
            {
                while (this.messageQueue.Count > 0)
                {
                    byte[] message = this.messageQueue.Dequeue();

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
                            client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, remainingLength == 0, CancellationToken.None).Wait();
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceWarning("Web Socket send fault.");
                            Trace.TraceError(ex.Message);
                            throw ex;

                        }
                    } while (remainingLength > 0);
                }
            }
            catch (Exception ex)
            {
                exception = ex;
                Trace.TraceWarning("Web Socket exception during send.");
                Trace.TraceError(ex.Message);
            }

            if (exception != null)
            {
                CloseAsync().Wait();

                if (OnClose != null)
                {
                    OnClose(this, "Client forced to close.");
                }

                throw exception;
            }
        }

        public async Task SendAsync(byte[] messageBytes)
        {
            this.messageQueue.Enqueue(messageBytes);
            Exception exception = null;
            try
            {
                while (this.messageQueue.Count > 0)
                {
                    byte[] message = this.messageQueue.Dequeue();

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
                            Trace.TraceWarning("Web Socket send fault.");
                            Trace.TraceError(ex.Message);
                            throw;

                        }
                    } while (remainingLength > 0);
                }
            }
            catch (Exception ex)
            {
                exception = ex;
                Trace.TraceWarning("Web Socket exception during send.");
                Trace.TraceError(ex.Message);
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
