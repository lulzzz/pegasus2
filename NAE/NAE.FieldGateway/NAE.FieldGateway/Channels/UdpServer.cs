using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NAE.FieldGateway.Channels
{
    public delegate void UpdMessageEventHandler(object sender, string message);

    public class UdpServer
    {
        public UdpServer(int listenPort, int sendPort)
        {

            deviceIPString = ConfigurationManager.AppSettings["deviceIP"];
            this.listenPort = listenPort;
            this.sendPort = sendPort;
            server = new UdpClient(listenPort);
            server.DontFragment = true;            

        }

        public event UpdMessageEventHandler OnReceive;

        private UdpClient server;
        private UdpClient client;
        private IPEndPoint endpoint;
        private int listenPort;
        private int sendPort;
        private string deviceIPString;

        public async Task RunAsync()
        {
            while (true)
            {                
                UdpReceiveResult result = await server.ReceiveAsync();
                string data = Encoding.UTF8.GetString(result.Buffer);

                //if (endpoint == null)
                //{
                //    endpoint = result.RemoteEndPoint;
                //    client = new UdpClient(endpoint);
                //    client.DontFragment = true;
                //}

                //process data
                if (OnReceive != null)
                {
                    OnReceive(this, data);
                }
            }

        }

        public async Task SendAsync(byte[] data)
        {
            try
            {
                if (client == null && deviceIPString == "NONE")
                {
                    MessageBox.Show("Cannot send UDP message as client is null, deviceIP is NONE, and no message was received to set endpoint.");
                    return;
                }

                if (client == null && deviceIPString != "NONE")
                {
                    endpoint = new IPEndPoint(IPAddress.Parse(deviceIPString), sendPort);
                    //client = new UdpClient(endpoint);
                    client = new UdpClient();
                    client.Connect(endpoint);
                }

                if (client == null)
                {
                    MessageBox.Show("UDP client is null and cannot send messages.");
                    return;
                }

                await client.SendAsync(data, data.Length);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "UDP");
            }
        }

        public void Send(byte[] data)
        {
            if (client == null && deviceIPString == "NONE")
            {
                MessageBox.Show("Cannot send UDP message as client is null, deviceIP is NONE, and no message was received to set endpoint.");
                return;
            }           

            if (client == null && deviceIPString != "NONE")
            {
                endpoint = new IPEndPoint(IPAddress.Parse(deviceIPString), sendPort);
                client = new UdpClient();
                client.Connect(endpoint);
                //client = new UdpClient(endpoint);
                
                //client = new UdpClient(new IPEndPoint(IPAddress.Parse(deviceIPString), port));
                
            }

            if (client == null)
            {
                MessageBox.Show("UDP client is null and cannot send messages.");
                return;
            }
            client.Send(data, data.Length);
        }


    }
}
