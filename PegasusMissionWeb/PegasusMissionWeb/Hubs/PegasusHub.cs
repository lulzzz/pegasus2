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
        private static string host = "ws://habtest.azurewebsites.net/api/connect";
        private static string subprotocol = "coap.v1";
        WebSocketClient client = new WebSocketClient();
        public bool webSocketInstantiated = false;

        public void Send(string name, string message)
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
                    await client.ConnectAsync(host, subprotocol, null);
                });
                message = "Sample Message - Hub Starting";
            }
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);

        }

        public void client_OnMessage(object sender, byte[] message)
        {
            CoapMessage coapMessage = CoapMessage.DecodeMessage(message);
            string jsonString = Encoding.UTF8.GetString(coapMessage.Payload);
            CraftTelemetry telemetry = JsonConvert.DeserializeObject<CraftTelemetry>(jsonString);
            //Console.WriteLine(telemetry.AtmosphericPressure);
            Send(telemetry.GpsLatitude.ToString(), telemetry.GpsLongitude.ToString() );
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