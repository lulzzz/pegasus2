using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;


using Piraeus.Protocols.Coap;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Pegasus2.Data;
using Piraeus.Web.WebSockets;
using Newtonsoft.Json;

namespace PegasusMissionWeb.Hubs
{
    public class PegasusHub : Hub
    {
        //private static string host = "ws://habtest.azurewebsites.net/api/connect";
        private static string host = "ws://broker.pegasusmission.io/api/connect";
        private static string subprotocol = "coap.v1";
        private static string tokenString = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJodHRwOi8vcGVnYXN1c21pc3Npb24uaW8vY2xhaW1zL25hbWUiOiIwZWFjNjZlZi1jMDNkLTQ1ZDQtYmNjMS01NzMyYjQyMTQ2YTQiLCJodHRwOi8vcGVnYXN1c21pc3Npb24uaW8vY2xhaW1zL3JvbGUiOiJ1c2VyIiwiaXNzIjoidXJuOnBlZ2FzdXNtaXNzaW9uLmlvIiwiYXVkIjoiaHR0cDovL2Jyb2tlci5wZWdhc3VzbWlzc2lvbi5pby9hcGkvY29ubmVjdCIsImV4cCI6MTQ2ODk1NjQ0NSwibmJmIjoxNDM3NDIwNDQ1fQ.VW85TZ_LrIvMQv3d7tVMFGJvE8M7H60MIe1xF8bcv1g";
        private static string telemetryUriString = "coaps://pegasusmission.io/subscribe?topic=http://pegasus2.org/telemetry";
        private static string groundUriString = "coaps://pegasusmission.io/subscribe?topic=http://pegasus2.org/ground";
        private static string noteUriString = "coaps://pegasusmission.io/subscribe?topic=http://pegasus2.org/craftnote";
        WebSocketClient client = new WebSocketClient();

        public bool webSocketInstantiated = false;

        private static bool opened;

        public void Send(string jsonString, string telemetryType)
        {
            if (webSocketInstantiated == false)
            {
                //WebSocketClient client = new WebSocketClient();
                webSocketInstantiated = true;
                client.OnError += client_OnError;
                client.OnOpen += client_OnOpen;
                client.OnClose += client_OnClose;
                client.OnMessage += client_OnMessage;
                Task task = Task.Factory.StartNew(async () =>
                {
                    await client.ConnectAsync(host, subprotocol, tokenString);
                });

                Task.WhenAll(task);

                while (!opened)
                {
                    Thread.Sleep(100);
                }

                Subscribe();
            }
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(jsonString, telemetryType);


        }



        private void Subscribe()
        {
            CoapRequest telemetrySubscription = new CoapRequest(1, RequestMessageType.NonConfirmable, MethodType.POST, new Uri(telemetryUriString), MediaType.Json);
            CoapRequest groundSubscription = new CoapRequest(2, RequestMessageType.NonConfirmable, MethodType.POST, new Uri(groundUriString), MediaType.Json);
            CoapRequest noteSubscription = new CoapRequest(3, RequestMessageType.NonConfirmable, MethodType.POST, new Uri(noteUriString), MediaType.Json);

            client.SendAsync(telemetrySubscription.Encode()).Wait();
            client.SendAsync(groundSubscription.Encode()).Wait();
            client.SendAsync(noteSubscription.Encode()).Wait();
        }

        public void client_OnMessage(object sender, byte[] message)
        {
            CoapMessage coapMessage = CoapMessage.DecodeMessage(message);
            string jsonString = Encoding.UTF8.GetString(coapMessage.Payload);

            if (coapMessage.ResourceUri.ToString().Contains("/telemetry"))
            {
                CraftTelemetry ct = CraftTelemetry.FromJson(jsonString);
                Send(jsonString, "balloon");
                //how does the map get updated for the craft???
            }
            else if (coapMessage.ResourceUri.ToString().Contains("/ground"))
            {
                //this is the data from the ground stations to use on the map

                GroundTelemetry gt = GroundTelemetry.FromJson(jsonString);
                if (gt.Source == "mobile") //the mobile ground unit
                {
                    Send(jsonString, "mobile");                    
                }
                else //the launch/stationary ground unit
                {
                    Send(jsonString, "launch");                   
                }

            }

            //Send(telemetry.GpsLatitude.ToString(), telemetry.GpsLongitude.ToString(), telemetry.Accelerometer.ToString() );
        }

        static void client_OnClose(object sender, string message)
        {
            opened = false;
        }

        static void client_OnOpen(object sender, string message)
        {
            opened = true;
        }

        static void client_OnError(object sender, Exception ex)
        {
            opened = false;
        }
    }
}