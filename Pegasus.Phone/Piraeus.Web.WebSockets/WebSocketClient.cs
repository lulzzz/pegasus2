using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using WebSocket4Net;

namespace Piraeus.Web.WebSockets
{
    public delegate void WebSocketEventHandler(object sender, string message);
    public delegate void WebSocketErrorHandler(object sender, Exception ex);
    public delegate void WebSocketMessageHandler(object sender, byte[] message);
    public class WebSocketClient
    {
        public WebSocketClient()
        {
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

        private const int receiveChunkSize = 1024;
        private WebSocket client;
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
            await Task.Run(() =>
            {
                this.client = new WebSocket(host, subprotocol ?? String.Empty);

                // TODO: securityToken!
                //                if (!string.IsNullOrEmpty(securityToken))
                //                {
                //                    client.Options.SetRequestHeader("Authorization", String.Format("Bearer {0}", securityToken));
                //                }

                this.client.DataReceived += Client_DataReceived;
                this.client.Open();

                if (OnOpen != null)
                {
                    OnOpen(this, "Web socket is opened.");
                }
            });
        }

       public async Task CloseAsync()
        {
            if (this.client != null)
            {
                await Task.Run(() =>
                {
                    this.client.Close();
                    this.client = null;

                    if (OnClose != null)
                    {
                        OnClose(this, "Web socket is closed.");
                    }
                });
            }
        }

        private void Client_DataReceived(object sender, WebSocket4Net.DataReceivedEventArgs e)
        {
            Exception exception = null;
            byte[] prefix = null;
            int offset = 0;
            int sourceOffset = 0;
            int remainingLength = 0;

            while(sourceOffset < e.Data.Length)
            {
                try
                {
                    if (prefix == null)
                    {
                        prefix = new byte[4];
                        Array.Copy(e.Data, sourceOffset, prefix, 0, 4);
                        sourceOffset += 4;
                        prefix = BitConverter.IsLittleEndian ? prefix.Reverse().ToArray() : prefix;
                        remainingLength = BitConverter.ToInt32(prefix, 0);
                    }
                    else
                    {
                        int index = 0;
                        byte[] message = new byte[remainingLength];
                        do // this whole loop is bad!
                        {
                            int bufferSize = remainingLength > receiveChunkSize ? receiveChunkSize : remainingLength;
                            Array.Copy(e.Data, sourceOffset, message, index, bufferSize);
                            sourceOffset += bufferSize;
                            index += bufferSize;
                            remainingLength = bufferSize - index; // TODO: doesn't seem right
                        } while (remainingLength > 0);

                        prefix = null;

                        if (OnMessage != null)
                        {
                            OnMessage(this, message);
                        }
                    }
                }
                catch(Exception ex)
                {
                    exception = ex;
//                    Trace.TraceWarning("Web socket receive faulted.");
//                    Trace.TraceError(ex.Message);
                }
            }

            if (exception != null)
            {
//                await CloseAsync();

                if (OnClose != null)
                {
                    OnClose(this, "Client forced to close.");
                }
            }
        }

        public async Task SendAsync(byte[] message)
        {
            await Task.Run(() =>
                {
                    client.Send(message, 0, message.Length);
                }
            );
#if false
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
                            Trace.TraceWarning("Web Socket send fault.");
                            Trace.TraceError(ex.Message);
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
#endif
        }
    }
}
