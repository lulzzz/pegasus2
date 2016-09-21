using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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
        public UdpServer(int port)
        {
            this.port = port;
            Setup();        

        }

        public event UpdMessageEventHandler OnReceive;

        private UdpClient server;
        private IPEndPoint endpoint;
        private int port;
        private string deviceIPString;

        public async Task RunAsync()
        {
            while (true)
            {
                byte[] dataArray = null;
                Exception fault = null;

                Task task = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        dataArray = server.Receive(ref endpoint);
                    }
                    catch(Exception ex)
                    {
                        fault = ex;
                    }
                });

                await Task.WhenAll(task);

                if (fault != null)
                {
                    Trace.TraceWarning("Fault in UDP Receive");
                    Trace.TraceError(fault.Message);
                    break;
                }
                else
                {
                    if (OnReceive != null)
                    {
                        OnReceive(this, Encoding.UTF8.GetString(dataArray));
                    }
                }
            }


            MessageBox.Show("UDP Stopped", "UDP");

        }

        public async Task SendAsync(byte[] data)
        {
            Exception fault = null;

            try
            {
                Task sendTask = Task.Factory.StartNew(() =>
                {
                    server.Send(data, data.Length, endpoint);
                });

                await Task.WhenAll(sendTask);
            }
            
            catch(Exception ex)
            {
                Trace.TraceWarning("Fault in UDP SendAsync");
                Trace.TraceError(ex.Message);
            }

            if (fault != null)
            {
                Setup();
                await RunAsync();
            }


        }

        public void Send(byte[] data)
        {
            Exception fault = null;

            try
            {
                server.Send(data, data.Length, endpoint);
            }
            catch(Exception ex)
            {
                Trace.TraceWarning("Fault in UDP Send");
                Trace.TraceError(ex.Message);               
                fault = ex;    
            }

            if(fault != null)
            {
                Setup();
                Task task = Task.Factory.StartNew(async () =>
                {
                    await RunAsync();
                });

                Task.WhenAll(task);
            }
            
            
        }


        private void Setup()
        {
            if(server != null)
            {
                server.Close();
                server = null;
            }

            deviceIPString = ConfigurationManager.AppSettings["deviceIP"];
            server = new UdpClient(this.port);
            endpoint = new IPEndPoint(IPAddress.Parse(deviceIPString), this.port);
        }

    }
}
