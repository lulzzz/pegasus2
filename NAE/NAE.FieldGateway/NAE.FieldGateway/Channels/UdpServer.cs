using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NAE.FieldGateway.Channels
{
    public delegate void UpdMessageEventHandler(object sender, string message);

    public class UdpServer
    {
        public UdpServer(int port)
        {
            server = new UdpClient(port);
            server.DontFragment = true;
        }

        public event UpdMessageEventHandler OnReceive;

        private UdpClient server;
        private UdpClient client;
        private IPEndPoint endpoint;
        public async Task RunAsync()
        {
            while (true)
            {
                UdpReceiveResult result = await server.ReceiveAsync();
                string data = Encoding.UTF8.GetString(result.Buffer);

                if (endpoint == null)
                {
                    endpoint = result.RemoteEndPoint;
                    client = new UdpClient(endpoint);
                    client.DontFragment = true;
                }

                //process data
                if (OnReceive != null)
                {
                    OnReceive(this, data);
                }
            }

        }

        public async Task SendAsync(byte[] data)
        {
            await client.SendAsync(data, data.Length);
        }

        public void Send(byte[] data)
        {
            client.Send(data, data.Length);
        }


    }
}
