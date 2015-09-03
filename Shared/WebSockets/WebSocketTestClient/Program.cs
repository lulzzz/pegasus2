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
        private static string UserMessageTopic = "http://pegasus2.org/usermessage";
        private static string JwtToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJodHRwOi8vcGVnYXN1c21pc3Npb24uaW8vY2xhaW1zL25hbWUiOiJkMjFjM2M2NC0xNmZiLTQwNTItYWRlMi1mZDY4YzcyYzhkMWQiLCJodHRwOi8vcGVnYXN1c21pc3Npb24uaW8vY2xhaW1zL3JvbGUiOlsidXNlciIsInNlcnZpY2UiXSwiaXNzIjoidXJuOnBlZ2FzdXNtaXNzaW9uLmlvIiwiYXVkIjoiaHR0cDovL2Jyb2tlci5wZWdhc3VzbWlzc2lvbi5pby9hcGkvY29ubmVjdCIsImV4cCI6MTQ3MjgzNDcxNywibmJmIjoxNDQxMjk4NzE3fQ.dZGKE-vW-cQicpr3ijYjEWGhQj_pRqLx1BPS0zMiKEw";

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
                    Subscribe(UserMessageTopic);
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
            if (coapMessage.ResourceUri.Query.Contains(TelemetryTopic))
            {
                CraftTelemetry telemetry = JsonConvert.DeserializeObject<CraftTelemetry>(jsonString);
                Console.WriteLine("Craft Telemetry: " + telemetry.AtmosphericPressure);
            }
            else if (coapMessage.ResourceUri.Query.Contains(GroundTopic))
            {
                GroundTelemetry telemetry = JsonConvert.DeserializeObject<GroundTelemetry>(jsonString);
                Console.WriteLine("Ground Telemetry: " + telemetry.ActualDistance);
            }
            else if (coapMessage.ResourceUri.Query.Contains(UserMessageTopic))
            {
                UserMessage userMessage = JsonConvert.DeserializeObject<UserMessage>(jsonString);
                Console.WriteLine("User Message: " + userMessage.Message);
            }
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
