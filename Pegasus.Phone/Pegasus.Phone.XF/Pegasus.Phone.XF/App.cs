using Newtonsoft.Json;
using Pegasus.Phone.XF.WebSocket;
using Pegasus2.Data;
using Piraeus.ServiceModel.Protocols.Coap;
//using Piraeus.Web.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pegasus.Phone.XF
{
	public class App : Application
	{
        private static string host = "ws://habtest.azurewebsites.net/api/connect";
        private static string subprotocol = "coap.v1";
        Label myLabel;

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
 
		public App ()
		{
            myLabel = new Label
            {
                XAlign = TextAlignment.Center,
                Text = "Welcome to Xamarin Forms!"
            };

            Button button = new Button { Text = "Go" };
            button.Clicked += ConnectWebSocket;
 
			// The root page of your application
			MainPage = new ContentPage {
				Content = new StackLayout {
					VerticalOptions = LayoutOptions.Center,
					Children = {
                        myLabel,
                        button
					}
				}
			};
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
