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
        private static string Host = "wss://broker.pegasusmission.io/api/connect";
        private static string SubProtocol = "coap.v1";
        private static string GroundTopicPublishUri = "coaps://pegasusmission.io/publish?topic=http://pegasus2.org/ground";
        private static string GroundTopicSubscribeUri = "coaps://pegasusmission.io/subscribe?topic=http://pegasus2.org/ground";
        private static string TelemetryTopicPublishUri = "coaps://pegasusmission.io/publish?topic=http://pegasus2.org/telemetry";
        private static string TelemetryTopicSubscribeUri = "coaps://pegasusmission.io/subscribe?topic=http://pegasus2.org/telemetry";
        private static string JwtToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJodHRwOi8vcGVnYXN1c21pc3Npb24uaW8vY2xhaW1zL25hbWUiOiJhYmMyIiwiaHR0cDovL3BlZ2FzdXNtaXNzaW9uLmlvL2NsYWltcy9yb2xlIjoidXNlciIsImlzcyI6InVybjpwZWdhc3VzbWlzc2lvbi5pbyIsImF1ZCI6Imh0dHA6Ly9icm9rZXIucGVnYXN1c21pc3Npb24uaW8vYXBpL2Nvbm5lY3QiLCJleHAiOjE0NjUyMDg5MDQsIm5iZiI6MTQzMzY3MjkwNH0.p856DcRRnGAwZJyPCbBSfrBY5Uwp21_4oNQcxNQamFI";

        private IWebSocketClient client;
        private static ushort messageId;

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

        public GroundTelemetryViewModel CurrentChaseTelemetry
        {
            get;
            private set;
        }

        public App()
        {
            Instance = this;
            AppData = new AppDataViewModel { StatusMessage = "Application launched, press go!" };
            CurrentCraftTelemetry = new CraftTelemetryViewModel();
            CurrentChaseTelemetry = new GroundTelemetryViewModel();
            MainPage = new MainPage();
        }

        public void ConnectWebSocket()
        {
            Device.BeginInvokeOnMainThread(() => this.AppData.StatusMessage = "Connecting...");

            Task task = Task.Factory.StartNew(() =>
            {
                messageId = 1;
                client = DependencyService.Get<IWebSocketClient>();
                client.OnError += client_OnError;
                client.OnOpen += client_OnOpen;
                client.OnClose += client_OnClose;
                client.OnMessage += client_OnMessage;
                client.ConnectAsync(Host, SubProtocol, JwtToken).Wait();
                SubscribeTopic(TelemetryTopicSubscribeUri);
                SubscribeTopic(GroundTopicSubscribeUri);
            });

            Task.WhenAll(task);
        }

        private void SubscribeTopic(string subscribeUri)
        {
            Uri resourceUri = new Uri(subscribeUri);
            CoapRequest request = new CoapRequest(messageId++, RequestMessageType.NonConfirmable, MethodType.POST, resourceUri, MediaType.Json);
            byte[] message = request.Encode();
            client.SendAsync(message).Wait();
            Device.BeginInvokeOnMainThread(() => this.AppData.StatusMessage = "Subscribed " + subscribeUri);
        }

        private void client_OnMessage(object sender, byte[] message)
        {
            CoapMessage coapMessage = CoapMessage.DecodeMessage(message);
            string jsonString = Encoding.UTF8.GetString(coapMessage.Payload, 0, coapMessage.Payload.Length);

            if (coapMessage.ResourceUri.OriginalString == GroundTopicPublishUri)
            {
                var telemetry = JsonConvert.DeserializeObject<GroundTelemetry>(jsonString);

                if (telemetry.Source != "mobile")
                {
                    return;
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    this.AppData.MessageCount++;
                    this.CurrentChaseTelemetry.Data = telemetry;
                });
            }
            else if (coapMessage.ResourceUri.OriginalString == TelemetryTopicPublishUri)
            {
                var telemetry = JsonConvert.DeserializeObject<CraftTelemetry>(jsonString);

                Device.BeginInvokeOnMainThread(() =>
                {
                    this.AppData.MessageCount++;
                    this.CurrentCraftTelemetry.Data = telemetry;
                });
            }
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


        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
