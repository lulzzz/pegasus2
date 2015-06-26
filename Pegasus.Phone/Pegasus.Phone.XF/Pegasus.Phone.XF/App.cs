
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
        private static string SubProtocol = "coap.v1";
        private static string Host = "ws://broker.pegasusmission.io/api/connect";
        private static string GroundTopicPublishUri = "coaps://pegasusmission.io/publish?topic=http://pegasus2.org/ground";
        private static string GroundTopicSubscribeUri = "coaps://pegasusmission.io/subscribe?topic=http://pegasus2.org/ground";
        private static string TelemetryTopicPublishUri = "coaps://pegasusmission.io/publish?topic=http://pegasus2.org/telemetry";
        private static string TelemetryTopicSubscribeUri = "coaps://pegasusmission.io/subscribe?topic=http://pegasus2.org/telemetry";
        private static string userMessageTopicUriString = "coaps://pegasusmission.io/publish?topic=http://pegasus2.org/usermessage";
        private static string JwtToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJodHRwOi8vcGVnYXN1c21pc3Npb24uaW8vY2xhaW1zL25hbWUiOiJhYmMyIiwiaHR0cDovL3BlZ2FzdXNtaXNzaW9uLmlvL2NsYWltcy9yb2xlIjoidXNlciIsImlzcyI6InVybjpwZWdhc3VzbWlzc2lvbi5pbyIsImF1ZCI6Imh0dHA6Ly9icm9rZXIucGVnYXN1c21pc3Npb24uaW8vYXBpL2Nvbm5lY3QiLCJleHAiOjE0NjUyMDg5MDQsIm5iZiI6MTQzMzY3MjkwNH0.p856DcRRnGAwZJyPCbBSfrBY5Uwp21_4oNQcxNQamFI";

        private IWebSocketClient client;
        private static ushort messageId;

        private static string fakeCraftTelemetryLine = "$:2015-01-28T21:49:18Z,989.6,198.8,13.0,77.6,13.0,2.2,7.5,7.4,0,0,1,0,-3200,-384,17408,-3200,-384,17408,-3200,-384,17408,1.0,46.8301,-119.1643,198.8,6.4,169.5,1,6,0,-0.7,0,0,1,0,0,1000,02:30,*CA";
        private static double fakeLaunchLatitude = 46.8422;
        private static double fakeLaunchLongitude = -119.1632;
        private static double fakeLaunchAltitude = 198.8;

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


        public GroundTelemetryViewModel CurrentLaunchTelemetry
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
            CurrentLaunchTelemetry = new GroundTelemetryViewModel();
            //MainPage = new RootPage();
            MainPage = new MainPage();
        }

        private static GroundTelemetry CreateGroundTelemetry(CraftTelemetry craftTelemetry, bool mobile)
        {
            GroundTelemetry gt = new GroundTelemetry();
            gt.Source = mobile ? "mobile" : "launch";  //telemetrySource;
            gt.Timestamp = craftTelemetry.Timestamp;
            gt.Temperature = new Random().Next(17, 25);


            if (mobile)
            {
                gt.GpsAltitude = new Random().Next(500, 600);
                gt.GpsLatitude = craftTelemetry.GpsLatitude + .0111;
                gt.GpsLongitude = craftTelemetry.GpsLongitude + 0.0111;
                gt.GpsDirection = new Random().Next(80, 110);
                gt.GpsSpeed = new Random().Next(10, 50);
            }
            else //launch 
            {
                gt.GpsAltitude = fakeLaunchAltitude;
                gt.GpsLatitude = fakeLaunchLatitude;
                gt.GpsLongitude = fakeLaunchLongitude;
                gt.Azimuth = new Random().Next(45, 120);
                gt.Elevation = new Random().Next(10, 75);
            }

            gt.GpsFix = true;
            gt.GpsSatellites = new Random().Next(1, 6);
            gt.RadioStrength = new Random().Next(50, 98);
            gt.ReceptionErrors = new Random().Next(0, 2);
            gt.BatteryLevel = new Random().NextDouble() * 7;

            Location groundLocation = new Location() { Latitude = gt.GpsLatitude, Longitude = gt.GpsLongitude };
            Location craftLocation = new Location() { Latitude = craftTelemetry.GpsLatitude, Longitude = craftTelemetry.GpsLongitude };
            Location fixedLocation = new Location() { Latitude = fakeLaunchLatitude, Longitude = fakeLaunchLongitude };

            double distKM = TrackingHelper.CalculateDistance(groundLocation, craftLocation);
            double distMI = distKM * 0.621371;
            gt.GroundDistance = distMI;
            gt.ActualDistance = (Math.Pow(Math.Pow(distKM, 2) + Math.Pow((craftTelemetry.GpsAltitude - gt.GpsAltitude) / 1000, 2), 0.5)) * 0.621371;

            if (mobile)
            {
                double distKMToMobile = TrackingHelper.CalculateDistance(groundLocation, fixedLocation);
                double distMIToMobile = distKM * 0.621371;
                gt.PeerDistance = distMIToMobile;
            }
            else
            {
                gt.PeerDistance = 0;
            }

            return gt;
        }

        public async Task ConnectWebSocket()
        {
            this.AppData.StatusMessage = "Connecting...";
            this.AppData.BusyCount++;

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

            await task;
        }

        public async Task FakeLocationAsync()
        {
            this.AppData.BusyCount++;

            await Task.Delay(250); // tiny delay to simulate a real connection
            this.AppData.MessageCount++;
            var craftTelemetry = (CraftTelemetry)PegasusMessage.Decode(fakeCraftTelemetryLine);
            this.CurrentCraftTelemetry.Data = craftTelemetry;
            this.CurrentChaseTelemetry.Data = CreateGroundTelemetry(craftTelemetry, true);
            this.CurrentLaunchTelemetry.Data = CreateGroundTelemetry(craftTelemetry, false);

            this.AppData.BusyCount--;
        }

        public async Task SendUserMessageAsync(string message)
        {
            this.AppData.StatusMessage = "Sending...";
            this.AppData.BusyCount++;

            await Task.Factory.StartNew(() =>
            {
                SendUserMessage(message);
            });
        }

        private void SendUserMessage(string message)
        {
            UserMessage umessage = new UserMessage();
            //adding ticks to the user message for testing latency (not used in production)
            umessage.Message = message + "_" + DateTime.UtcNow.Ticks.ToString();
            umessage.id = Guid.NewGuid().ToString();
            string jsonString = umessage.ToJson();
            byte[] payload = Encoding.UTF8.GetBytes(jsonString);
            CoapRequest request = new CoapRequest(messageId++,
                                                    RequestMessageType.NonConfirmable,
                                                    MethodType.POST,
                                                    new Uri(userMessageTopicUriString),
                                                    MediaType.Json,
                                                    payload);

            byte[] messageBytes = request.Encode();
            client.SendAsync(messageBytes).Wait();
            Device.BeginInvokeOnMainThread(() => 
            { 
                this.AppData.StatusMessage = "Message Sent!";
                this.AppData.BusyCount--;
            });
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

                Device.BeginInvokeOnMainThread(() =>
                {
                    this.AppData.MessageCount++;
                    if (telemetry.Source == "mobile")
                    {
                        this.CurrentChaseTelemetry.Data = telemetry;
                    }
                    else
                    {
                        this.CurrentLaunchTelemetry.Data = telemetry;
                    }
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
            Device.BeginInvokeOnMainThread(() =>
            {
                this.AppData.StatusMessage = "Web Socket is open";
                this.AppData.BusyCount--;
            });
        }

        private void client_OnError(object sender, Exception ex)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                this.AppData.StatusMessage = "Web Socket error: " + ex.Message;
                this.AppData.BusyCount--;
            });
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

        internal class Location
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        internal static class TrackingHelper
        {
            private static double DegreesToRadians(double degrees)
            {
                return degrees * Math.PI / 180.0;
            }

            public static double CalculateDistance(Location location1, Location location2)
            {
                double circumference = 40000.0; // Earth's circumference at the equator in km
                double distance = 0.0;

                //Calculate radians
                double latitude1Rad = DegreesToRadians(location1.Latitude);
                double longitude1Rad = DegreesToRadians(location1.Longitude);
                double latititude2Rad = DegreesToRadians(location2.Latitude);
                double longitude2Rad = DegreesToRadians(location2.Longitude);

                double logitudeDiff = Math.Abs(longitude1Rad - longitude2Rad);

                if (logitudeDiff > Math.PI)
                {
                    logitudeDiff = 2.0 * Math.PI - logitudeDiff;
                }

                double angleCalculation =
                    Math.Acos(
                      Math.Sin(latititude2Rad) * Math.Sin(latitude1Rad) +
                      Math.Cos(latititude2Rad) * Math.Cos(latitude1Rad) * Math.Cos(logitudeDiff));

                distance = circumference * angleCalculation / (2.0 * Math.PI);

                return distance;
            }

            public static double CalculateDistance(params Location[] locations)
            {
                double totalDistance = 0.0;

                for (int i = 0; i < locations.Length - 1; i++)
                {
                    Location current = locations[i];
                    Location next = locations[i + 1];

                    totalDistance += CalculateDistance(current, next);
                }

                return totalDistance;
            }
        }
    }
}
