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
using Pegasus.Phone.XF.ViewModels;
//using Piraeus.Web.WebSockets;

namespace Pegasus.Phone.XF.Pages
{
    public partial class MainPage : ContentPage
    {
        private static string host = "ws://habtest.azurewebsites.net/api/connect";
        private static string subprotocol = "coap.v1";

        private MainPageViewModel ViewModel;

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

        private void client_OnMessage(object sender, byte[] message)
        {
            CoapMessage coapMessage = CoapMessage.DecodeMessage(message);
            string jsonString = Encoding.UTF8.GetString(coapMessage.Payload, 0, coapMessage.Payload.Length);
            var telemetry = JsonConvert.DeserializeObject<CraftTelemetry>(jsonString);
            Device.BeginInvokeOnMainThread(() =>
                {
                    this.ViewModel.MessageCount++;
                    this.ViewModel.CraftTelemetry = telemetry;
                });
        }

        private void client_OnClose(object sender, string message)
        {
            throw new NotImplementedException();
        }

        private void client_OnOpen(object sender, string message)
        {
            Device.BeginInvokeOnMainThread(() => this.ViewModel.StatusMessage = "Web Socket is open");
        
        }

        private void client_OnError(object sender, Exception ex)
        {
            throw new NotImplementedException();
        }
 
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = this.ViewModel = new MainPageViewModel();
        }
    }
}
