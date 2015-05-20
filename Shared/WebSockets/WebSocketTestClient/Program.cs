using Newtonsoft.Json;
using Pegasus2.Data;
using Piraeus.ServiceModel.Protocols.Coap;
using Piraeus.Web.WebSockets;
using Piraeus.Web.WebSockets.Net45;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketTestClient
{
    class Program
    {
        private static string host = "ws://habtest.azurewebsites.net/api/connect";
        private static string subprotocol = "coap.v1";
        static void Main(string[] args)
        {
            Console.WriteLine("press any key to start");
            Console.ReadKey();

            IWebSocketClient client = new WebSocketClient_Net45();
            client.OnError += client_OnError;
            client.OnOpen += client_OnOpen;
            client.OnClose += client_OnClose;
            client.OnMessage += client_OnMessage;
            Task task = Task.Factory.StartNew(async () =>
                {
                    await client.ConnectAsync(host, subprotocol, null);
                });

            Task.WhenAll(task);

            Thread.Sleep(30000);

            Console.WriteLine("Terminated");
            Console.ReadKey();


        }

        static void client_OnMessage(object sender, byte[] message)
        {
            CoapMessage coapMessage = CoapMessage.DecodeMessage(message);
            string jsonString = Encoding.UTF8.GetString(coapMessage.Payload);
            CraftTelemetry telemetry = JsonConvert.DeserializeObject<CraftTelemetry>(jsonString);
            Console.WriteLine(telemetry.AtmosphericPressure);

        }

        static void client_OnClose(object sender, string message)
        {
            throw new NotImplementedException();
        }

        static void client_OnOpen(object sender, string message)
        {
            Console.WriteLine("Web Socket is open");
        
        }

        static void client_OnError(object sender, Exception ex)
        {
            throw new NotImplementedException();
        }
    }
}
