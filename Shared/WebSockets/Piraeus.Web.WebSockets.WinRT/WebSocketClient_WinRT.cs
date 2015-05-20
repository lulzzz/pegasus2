using Piraeus.Web.WebSockets;
using Piraeus.Web.WebSockets.WinRT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private StreamWebSocket client;
        private DataWriter messageWriter;
        private DataReader messageReader;
        private bool connected = false;

        public event WebSocketEventHandler OnOpen;
        public event WebSocketEventHandler OnClose;
        public event WebSocketErrorHandler OnError;
        public event WebSocketMessageHandler OnMessage;

        private Queue<byte[]> messageQueue;

        public WebSocketClient_WinRT()
        {
            this.client = new StreamWebSocket();
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

            await ReceiveAsync();
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

        public async Task ReceiveAsync()
        {
            Exception exception = null;
            byte[] prefix = null;
            int offset = 0;
            int remainingLength = 0;

            while(connected)
            {
                try
                {
                    messageReader = new DataReader(client.InputStream);
                    messageReader.InputStreamOptions = InputStreamOptions.Partial;
                    if (prefix == null)
                    {
                        prefix = new byte[4];
                        //result = await client.ReceiveAsync(new ArraySegment<byte>(prefix), CancellationToken.None);
                        await messageReader.LoadAsync(4);
                        messageReader.ReadBytes(prefix);
                        prefix = BitConverter.IsLittleEndian ? prefix.Reverse().ToArray() : prefix;
                        remainingLength = BitConverter.ToInt32(prefix, 0);
                    }
                    else
                    {
                        int index = 0;
                        byte[] message = new byte[remainingLength];
                        do
                        {
                            uint bufferSize =(uint) (remainingLength > receiveChunkSize ? receiveChunkSize : remainingLength);
                            byte[] buffer = new byte[bufferSize];
                            await messageReader.LoadAsync(bufferSize);
                            messageReader.ReadBytes(buffer);
                            
                            System.Buffer.BlockCopy(buffer, 0, message, index, buffer.Length);
                            index += (int) bufferSize;
                            remainingLength = buffer.Length - index;
                        } while (remainingLength > 0);


                        prefix = null;

                        if(OnMessage != null)
                        {
                            OnMessage(this, message);
                        }
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
        }

        public async Task SendAsync(byte[] message)
        {
            Exception exception = null;
            try
            {
                messageWriter = new DataWriter(this.client.OutputStream);
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
