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
using PegasusMissionWeb.Security;
using System.Security.Claims;
using System.Diagnostics;

namespace PegasusMissionWeb.Hubs
{
    public class PegasusHub : Hub
    {

        //private static string host = "ws://habtest.azurewebsites.net/api/connect";
        //private string host = "ws://broker.pegasusmission.io/api/connect";
        private string host = "ws://localhost:11748/api/connect";
        private string subprotocol = "coap.v1";
        //private static string tokenString = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJodHRwOi8vcGVnYXN1c21pc3Npb24uaW8vY2xhaW1zL25hbWUiOiIwZWFjNjZlZi1jMDNkLTQ1ZDQtYmNjMS01NzMyYjQyMTQ2YTQiLCJodHRwOi8vcGVnYXN1c21pc3Npb24uaW8vY2xhaW1zL3JvbGUiOiJ1c2VyIiwiaXNzIjoidXJuOnBlZ2FzdXNtaXNzaW9uLmlvIiwiYXVkIjoiaHR0cDovL2Jyb2tlci5wZWdhc3VzbWlzc2lvbi5pby9hcGkvY29ubmVjdCIsImV4cCI6MTQ2ODk1NjQ0NSwibmJmIjoxNDM3NDIwNDQ1fQ.VW85TZ_LrIvMQv3d7tVMFGJvE8M7H60MIe1xF8bcv1g";
        private string telemetryUriString = "coaps://pegasusmission.io/subscribe?topic=http://pegasus2.org/telemetry";
        private string groundUriString = "coaps://pegasusmission.io/subscribe?topic=http://pegasus2.org/ground";
        private string noteUriString = "coaps://pegasusmission.io/subscribe?topic=http://pegasus2.org/craftnote";


        private string issuer = "urn:pegasusmission.io";
        private string audience = "http://broker.pegasusmission.io/api/connect";
        private string signingKey = "cW0iA3P/mhFi0/O4EAja7UuJ16q6Aeg4cOzL7SIvLL8=";

        private WebSocketClient client;

        public bool webSocketInstantiated = false;

        private bool opened;

        public void Send(string jsonString, string telemetryType)
        {
            if (webSocketInstantiated == false)
            {
                Task task = Task.Factory.StartNew(async () =>
                {
                    if(client != null)
                    {
                        await client.CloseAsync();
                    }

                    client = new WebSocketClient();
                    webSocketInstantiated = true;
                    client.OnError += client_OnError;
                    client.OnOpen += client_OnOpen;
                    client.OnClose += client_OnClose;
                    client.OnMessage += client_OnMessage;

                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim("http://pegasusmission.io/claims/name", Guid.NewGuid().ToString()));
                    claims.Add(new Claim("http://pegasusmission.io/claims/role", "user"));

                    string tokenString = JwtSecurityTokenBuilder.Create(issuer, audience, claims, 20000, signingKey);
                
                
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
            Random ran = new Random();
            ushort id = (ushort)ran.Next(1, Convert.ToInt32(ushort.MaxValue - 10));
            CoapRequest telemetrySubscription = new CoapRequest(id++, RequestMessageType.NonConfirmable, MethodType.POST, new Uri(telemetryUriString), MediaType.Json);
            CoapRequest groundSubscription = new CoapRequest(id++, RequestMessageType.NonConfirmable, MethodType.POST, new Uri(groundUriString), MediaType.Json);
            CoapRequest noteSubscription = new CoapRequest(id++, RequestMessageType.NonConfirmable, MethodType.POST, new Uri(noteUriString), MediaType.Json);

            byte[] telemetrySubscribe = telemetrySubscription.Encode();
            byte[] groundSubscribe = groundSubscription.Encode();
            byte[] noteSubscribe = noteSubscription.Encode();

            client.Send(groundSubscribe);
            client.Send(telemetrySubscribe);
            client.Send(noteSubscribe);
            //client.SendAsync(groundSubscribe).Wait();
            //client.SendAsync(telemetrySubscribe).Wait();
            //client.SendAsync(noteSubscribe).Wait();


            //Task task1 = Task.Factory.StartNew(async () =>
            //    {
            //        await client.SendAsync(telemetrySubscribe);
            //    });

            //Task.WhenAll(task1);

            //Task task2 = Task.Factory.StartNew(async () =>
            //{
            //    await client.SendAsync(groundSubscribe);
            //});

            //Task.WhenAll(task2);


            //Task task3 = Task.Factory.StartNew(async () =>
            //{
            //    await client.SendAsync(noteSubscribe);
            //});

            //Task.WhenAll(task3);
        }


        public void client_OnMessage(object sender, byte[] message)
        {
            CoapMessage coapMessage = CoapMessage.DecodeMessage(message);
            //Trace.TraceInformation(coapMessage.ResourceUri.ToString());
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

        void client_OnClose(object sender, string message)
        {
            opened = false;
            webSocketInstantiated = false;
        }

        void client_OnOpen(object sender, string message)
        {
            opened = true;
        }

        void client_OnError(object sender, Exception ex)
        {
            opened = false;
            
        }
    }
}