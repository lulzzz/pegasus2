using Newtonsoft.Json;
using Pegasus2.Data;
using Piraeus.Protocols.Coap;
using Piraeus.Web.WebSockets;
using Piraeus.Web.WebSockets.Net45;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketCoapReceiver
{
    class Program
    {
        //private static string host = "ws://localhost:11748/api/connect";
        private static string host = "wss://broker.pegasusmission.io/api/connect";
        private static string subprotocol = "coap.v1";
        private static string securityTokenString;
        private static string issuer = "urn:pegasusmission.io";
        private static string audience = "http://broker.pegasusmission.io/api/connect";
        private static string signingKey = "cW0iA3P/mhFi0/O4EAja7UuJ16q6Aeg4cOzL7SIvLL8=";
        private static bool opened;
        private static ushort messageId;
        private static string filename;

        private static string craftTelemetryTopic = "http://pegasus2.org/telemetry";
        private static string groundTelemetryTopic = "http://pegasus2.org/ground";
        private static string userMessageTopic = "http://pegasus2.org/usermessage";
        private static string coapAuthority = "pegasusmission.io";

        static void Main(string[] args)
        {
            Console.WriteLine("Receiver for Craft + Gronud Telemetry and User Messages");
            filename = Guid.NewGuid().ToString();
            messageId++;
            Console.WriteLine("Web Socket CoAP receiver press any key");
            Console.WriteLine();
            Console.ReadKey();

            SetSecurityToken();
            IWebSocketClient client = new WebSocketClient_Net45();
            client.OnClose += client_OnClose;
            client.OnError += client_OnError;
            client.OnOpen += client_OnOpen;
            client.OnMessage += client_OnMessage;

            Task task = Task.Factory.StartNew(async () =>
            {
                await client.ConnectAsync(host, subprotocol, securityTokenString);
            });

            Task.WhenAll(task);

            while (!opened)
            {
                Thread.Sleep(100);
            }

            Console.WriteLine("Subscribing to topics");
            Subscribe(client);


            Console.WriteLine("Press any key to terminate...");
            Console.ReadKey();
        }

        private static void Subscribe(IWebSocketClient client)
        {
            Uri craftTelemetryResourceUri = new Uri(String.Format("coaps://{0}/subscribe?topic={1}", coapAuthority, craftTelemetryTopic));
            Uri groundTelemetryResourceUri = new Uri(String.Format("coaps://{0}/subscribe?topic={1}", coapAuthority, groundTelemetryTopic));
            Uri userMessageResourceUri = new Uri(String.Format("coaps://{0}/subscribe?topic={1}", coapAuthority, userMessageTopic));

            //subscribe to craft telemetry
            CoapRequest request1 = new CoapRequest(messageId++, RequestMessageType.NonConfirmable, MethodType.POST, craftTelemetryResourceUri, MediaType.Json);
            byte[] messageBytes1 = request1.Encode();
            client.SendAsync(messageBytes1).Wait();

            ////subscribe to ground telemetry
            CoapRequest request2 = new CoapRequest(messageId++, RequestMessageType.NonConfirmable, MethodType.POST, groundTelemetryResourceUri, MediaType.Json);
            byte[] messageBytes2 = request2.Encode();
            client.SendAsync(messageBytes2).Wait();

            //subscribe to user message
            CoapRequest request3 = new CoapRequest(messageId++, RequestMessageType.NonConfirmable, MethodType.POST, userMessageResourceUri, MediaType.Json);
            byte[] messageBytes3 = request3.Encode();
            client.SendAsync(messageBytes3).Wait();
        }

        private static void SetSecurityToken()
        {
            string user = Guid.NewGuid().ToString(); //name of the user
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("http://pegasusmission.io/claims/name", user));
            claims.Add(new Claim("http://pegasusmission.io/claims/role", "user"));

            //adding another role to receive user messages
            claims.Add(new Claim("http://pegasusmission.io/claims/role", "service"));
            securityTokenString = JwtSecurityTokenBuilder.Create(issuer, audience, claims, 60 * 24 * 365, signingKey);
        }

        static void client_OnMessage(object sender, byte[] message)
        {
            long now = DateTime.UtcNow.Ticks;
            CoapMessage cmessage = CoapMessage.DecodeMessage(message);

            if(cmessage.ResourceUri.Query.Contains(craftTelemetryTopic))
            {
                //craft telemetry
                CraftTelemetry ct = new CraftTelemetry();
                CraftTelemetry ct2 = ct.FromJson(Encoding.UTF8.GetString(cmessage.Payload)) as CraftTelemetry;
                //write something out
                Console.WriteLine("Craft Telemetry Air Pressure {0}", ct2.AtmosphericPressure);
            }
            else if (cmessage.ResourceUri.Query.Contains(groundTelemetryTopic))
            {
                //ground telemetry
                GroundTelemetry gt = new GroundTelemetry();
                GroundTelemetry gt2 = gt.FromJson(Encoding.UTF8.GetString(cmessage.Payload)) as GroundTelemetry;
                //write something out
                Console.WriteLine("Ground Telemetry Source {0} Actual Distance {1}", gt2.Source, gt2.ActualDistance);
            }
            else if(cmessage.ResourceUri.Query.Contains(userMessageTopic))
            {
                //user message
                //adding for testing adding "ticks" to the user message in the form
                // <message>_<ticks>
                //to determine end-2-end latency (not used in production, just <message>)
                UserMessage um = new UserMessage();
                UserMessage um2 = um.FromJson(Encoding.UTF8.GetString(cmessage.Payload)) as UserMessage;
                string[] parts = um2.Message.Split('_');
                long start = Convert.ToInt64(parts[1]);
                TimeSpan diff = TimeSpan.FromTicks(now) - TimeSpan.FromTicks(start);
                Console.WriteLine("{0} {1}ms", parts[0], Math.Round(diff.TotalMilliseconds,0));
               
            }
            else
            {
                Console.WriteLine("Did not understand CoAP Resource URI");
            }
        }

        static void client_OnOpen(object sender, string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Ready to receive messages...");
            Console.ResetColor();
            opened = true;
        }

        static void client_OnError(object sender, Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            if(ex.InnerException != null)
            {
                Console.WriteLine(ex.InnerException.Message);
            }

            Console.ResetColor();

            Console.WriteLine("Press any key to terminate...");
            Console.ReadKey();
        }

        static void client_OnClose(object sender, string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();

            Console.WriteLine("Press any key to terminate...");
            Console.ReadKey();
        }
    }
}
