using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using Newtonsoft.Json;
using Piraeus.Web.WebSockets;
using Pegasus2.Data;
using Piraeus.ServiceModel.Protocols.Coap;
//using Piraeus.Web.WebSockets;

namespace Pegasus.Phone.XF.Pages
{
    public partial class MainPage : ContentPage
    {
        private static string host = "ws://habtest.azurewebsites.net/api/connect";
        private static string subprotocol = "coap.v1";

        public string MainText
        {
            get; set;
        }

        private void ConnectWebSocket(object sender, EventArgs e)
        {
            //WebSocketClient client = new WebSocketClient();
            var client = DependencyService.Get<IWebSocketClient>();
            client.OnError += client_OnError;
            client.OnOpen += client_OnOpen;
            client.OnClose += client_OnClose;
            client.OnMessage += client_OnMessage;
            Task task = Task.Factory.StartNew(async () =>
                {
                    await client.ConnectAsync(host, subprotocol, null);
                });

            Task.WhenAll(task);
        }

        private static int messageCount;

        private void client_OnMessage(object sender, byte[] message)
        {
            CoapMessage coapMessage = CoapMessage.DecodeMessage(message);
            string jsonString = Encoding.UTF8.GetString(coapMessage.Payload, 0, coapMessage.Payload.Length);
            CraftTelemetry telemetry = JsonConvert.DeserializeObject<CraftTelemetry>(jsonString);
            Device.BeginInvokeOnMainThread(() => myLabel.Text = String.Format("{0} ({1} messages)", telemetry.AtmosphericPressure, ++messageCount));
        }

        private void client_OnClose(object sender, string message)
        {
            throw new NotImplementedException();
        }

        private void client_OnOpen(object sender, string message)
        {
            Device.BeginInvokeOnMainThread(() => myLabel.Text = "Web Socket is open");
        
        }

        private void client_OnError(object sender, Exception ex)
        {
            throw new NotImplementedException();
        }
 
        public MainPage()
        {
            InitializeComponent();
            this.MainText = "Hello, world";
            this.BindingContext = this;
        }
    }
}
