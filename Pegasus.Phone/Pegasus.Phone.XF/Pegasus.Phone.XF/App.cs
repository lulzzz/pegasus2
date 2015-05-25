using Pegasus.Phone.XF.ViewModels;
using System;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

// Reference https://github.com/jamesmontemagno/Hanselman.Forms

using Newtonsoft.Json;
using Piraeus.Web.WebSockets;
using Pegasus2.Data;
using Piraeus.ServiceModel.Protocols.Coap;
using Pegasus.Phone.XF.ViewModels.Views;

namespace Pegasus.Phone.XF
{
	public class App : Application
	{
        private static string host = "ws://habtest.azurewebsites.net/api/connect";
        private static string subprotocol = "coap.v1";

        public AppDataViewModel AppData
        {
            get;
            private set;
        }

        public static App Instance
        {
            get;
            private set;
        }

        public CraftTelemetryViewModel CurrentCraftTelemetry
        {
            get;
            private set;
        }

        public GroundTelemetryViewModel CurrentGroundTelemetry
        {
            get;
            private set;
        }

        public App()
        {
            Instance = this;
            AppData = new AppDataViewModel { StatusMessage = "Application launched, press go!" };
            CurrentCraftTelemetry = new CraftTelemetryViewModel();
            CurrentGroundTelemetry = new GroundTelemetryViewModel();
            MainPage = new MainPage();
        }

        public void ConnectWebSocket()
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

            // TEMP apply geo drift
            telemetry.GpsLongitude += this.AppData.MessageCount / 100.0;
            telemetry.GpsLatitude += this.AppData.MessageCount / 50.0;

            // TEMP create ground telemetry
            GroundTelemetry groundTelemetry = null;
            if (CurrentGroundTelemetry.Data == null)
            {
                groundTelemetry = new GroundTelemetry
                {
                    GpsLatitude = telemetry.GpsLatitude,
                    GpsLongitude = telemetry.GpsLongitude
                };
            }

            Device.BeginInvokeOnMainThread(() =>
                {
                    this.AppData.MessageCount++;
                    this.CurrentCraftTelemetry.Data = telemetry;
                    if (groundTelemetry != null)
                    {
                        this.CurrentGroundTelemetry.Data = groundTelemetry;
                    }
                });
        }

        private void client_OnClose(object sender, string message)
        {
            throw new NotImplementedException();
        }

        private void client_OnOpen(object sender, string message)
        {
            Device.BeginInvokeOnMainThread(() => this.AppData.StatusMessage = "Web Socket is open");
        
        }

        private void client_OnError(object sender, Exception ex)
        {
            throw new NotImplementedException();
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
