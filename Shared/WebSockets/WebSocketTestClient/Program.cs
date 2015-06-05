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
        private static string Host = "wss://broker.pegasusmission.io/api/connect";
        private static string SubProtocol = "coap.v1";
        private static string TelemetryTopic = "http://pegasus2.org/telemetry";
        private static string GroundTopic = "http://pegasus2.org/ground";
        private static string JwtToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJodHRwOi8vcGVnYXN1c21pc3Npb24uaW8vY2xhaW1zL25hbWUiOiJhYmMyIiwiaHR0cDovL3BlZ2FzdXNtaXNzaW9uLmlvL2NsYWltcy9yb2xlIjoidXNlciIsImlzcyI6InVybjpwZWdhc3VzbWlzc2lvbi5pbyIsImF1ZCI6Imh0dHA6Ly9icm9rZXIucGVnYXN1c21pc3Npb24uaW8vYXBpL2Nvbm5lY3QiLCJleHAiOjE0NjUyMDg5MDQsIm5iZiI6MTQzMzY3MjkwNH0.p856DcRRnGAwZJyPCbBSfrBY5Uwp21_4oNQcxNQamFI";

        private static IWebSocketClient client;
        private static ushort messageId;

        static void Main(string[] args)
        {
            Console.WriteLine("press any key to start");
            Console.ReadKey();

            client = new WebSocketClient_Net45();
            client.OnError += client_OnError;
            client.OnOpen += client_OnOpen;
            client.OnClose += client_OnClose;
            client.OnMessage += client_OnMessage;
            Task task = Task.Factory.StartNew(() =>
                {
                    messageId = 1;
                    client.ConnectAsync(Host, SubProtocol, JwtToken).Wait();
                    Subscribe(TelemetryTopic);
                    Subscribe(GroundTopic);
                });

            Task.WhenAll(task);

            Thread.Sleep(30000);

            Console.WriteLine("Terminated");
            Console.ReadKey();
        }

        static void Subscribe(string topicUri)
        {
            Uri resourceUri = new Uri(String.Format("coaps://pegasusmission.io/subscribe?topic={0}", topicUri));
            CoapRequest request = new CoapRequest(messageId++, RequestMessageType.NonConfirmable, MethodType.POST, resourceUri, MediaType.Json);
            byte[] message = request.Encode();
            client.SendAsync(message).Wait();
            Console.WriteLine("Subscribed to {0}", topicUri);
        }

        static void client_OnMessage(object sender, byte[] message)
        {
            CoapMessage coapMessage = CoapMessage.DecodeMessage(message);
            string jsonString = Encoding.UTF8.GetString(coapMessage.Payload);
            // TODO: see which message this is, and process accordingly!
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
