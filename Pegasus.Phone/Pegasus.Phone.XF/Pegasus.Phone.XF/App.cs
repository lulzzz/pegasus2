
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
using System.Net;
using System.IO;
using Pegasus.Phone.XF.Utilities;

namespace Pegasus.Phone.XF
{
    public class App : Application
    {
        private const string SubProtocol = "coap.v1";
        private const string Host = "ws://broker.pegasusmission.io/api/connect";
        private const string GroundTopicPublishUri = "coaps://pegasusmission.io/publish?topic=http://pegasus2.org/ground";
        private const string GroundTopicSubscribeUri = "coaps://pegasusmission.io/subscribe?topic=http://pegasus2.org/ground";
        private const string TelemetryTopicPublishUri = "coaps://pegasusmission.io/publish?topic=http://pegasus2.org/telemetry";
        private const string TelemetryTopicSubscribeUri = "coaps://pegasusmission.io/subscribe?topic=http://pegasus2.org/telemetry";
        private const string UserMessageTopicUri = "coaps://pegasusmission.io/publish?topic=http://pegasus2.org/usermessage";
        private const string TokenSecret = "851o2LqnMUod9lp7DvVxSrH+KQAkydBF9MDREicDus4=";
        private const string TokenWebApiUri = "https://authz.pegasusmission.io/api/phone";
        private const string SavedSecurityTokenPrefix = "001;";
        private const string LaunchInfoUri = "http://pegasus2.blob.core.windows.net/info/launchinfo.json";

        private const string FakeCraftTelemetryLine = "$:2015-01-28T21:49:18Z,989.6,198.8,13.0,77.6,13.0,2.2,7.5,7.4,0,0,1,0,-3200,-384,17408,-3200,-384,17408,-3200,-384,17408,1.0,46.8301,-119.1643,198.8,6.4,169.5,1,6,0,-0.7,0,0,1,0,0,1000,02:30,*CA";
        private const double FakeLaunchLatitude = 46.8422;
        private const double FakeLaunchLongitude = -119.1632;
        private const double FakeLaunchAltitude = 198.8;

        private const double SecondsBetweenConnects = 20;

        private string jwtToken;

        private DateTime lastConnectAttemptTime;
        private IWebSocketClient client;
        private ushort messageId;

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
            AppData = new AppDataViewModel();
            CurrentCraftTelemetry = new CraftTelemetryViewModel();
            CurrentChaseTelemetry = new GroundTelemetryViewModel();
            CurrentLaunchTelemetry = new GroundTelemetryViewModel();
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
                gt.GpsAltitude = FakeLaunchAltitude;
                gt.GpsLatitude = FakeLaunchLatitude;
                gt.GpsLongitude = FakeLaunchLongitude;
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
            Location fixedLocation = new Location() { Latitude = FakeLaunchLatitude, Longitude = FakeLaunchLongitude };

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

        private async Task LoadSecurityToken()
        {
            if (jwtToken != null)
            {
                return;
            }

            string savedToken = Settings.SavedSecurityToken;
            if (savedToken != null)
            {
                if (savedToken.StartsWith(SavedSecurityTokenPrefix))
                {
                    jwtToken = savedToken.Substring(SavedSecurityTokenPrefix.Length);
                    return;
                }
            }

            //remember this is going to be a JSON string for the security token
            //so be sure to decode to a proper string; 
            string requestUriString = String.Format("{0}?key={1}", TokenWebApiUri, TokenSecret);

            var request = WebRequest.CreateHttp(requestUriString);
            WebResponse responseObject = await Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, request);
            using (var responseStream = responseObject.GetResponseStream())
            {
                using (var sr = new StreamReader(responseStream))
                {
                    string jsonString = await sr.ReadToEndAsync();
                    jwtToken = JsonConvert.DeserializeObject<string>(jsonString);
                    Settings.SavedSecurityToken = SavedSecurityTokenPrefix + jwtToken;
                }
            }
        }

        public async Task<LaunchInfo> GetLaunchInfoAsync()
        {
            var request = WebRequest.CreateHttp(LaunchInfoUri);
            WebResponse responseObject = await Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, request);
            using (var responseStream = responseObject.GetResponseStream())
            {
                using (var sr = new StreamReader(responseStream))
                {
                    string jsonString = await sr.ReadToEndAsync();
                    if (jsonString.StartsWith("\""))
                    {
                        jsonString = JsonConvert.DeserializeObject<String>(jsonString);
                    }
                    this.AppData.LaunchInfo = JsonConvert.DeserializeObject<LaunchInfo>(jsonString);
                    return this.AppData.LaunchInfo;
                }
            }
        }

        public async Task ConnectWebSocketAsync()
        {
            if (this.client != null)
            {
                return;
            }

            this.AppData.BusyCount++;

            if (jwtToken == null)
            {
                await LoadSecurityToken();
            }

            DateTime connectTime = lastConnectAttemptTime.AddSeconds(SecondsBetweenConnects);
            while (connectTime > DateTime.Now)
            {
                this.AppData.StatusMessage = "Delaying " + (int)(connectTime - DateTime.Now).TotalSeconds + " seconds before reconnecting...";
                await Task.Delay(1000);
            }

            this.AppData.StatusMessage = "Connecting...";

            messageId = 1;
            this.lastConnectAttemptTime = DateTime.Now;
            this.client = DependencyService.Get<IWebSocketClient>(DependencyFetchTarget.NewInstance);
            this.client.OnError += client_OnError;
            this.client.OnMessage += client_OnMessage;
            try
            {
                await this.client.ConnectAsync(Host, SubProtocol, jwtToken);
                await this.SubscribeTopicAsync(TelemetryTopicSubscribeUri);
                await this.SubscribeTopicAsync(GroundTopicSubscribeUri);
            }
            catch (Exception e)
            {
                this.client_OnError(client, e);
            }
            finally
            {
                this.AppData.BusyCount--;
            }
        }

        public async Task FakeLocationAsync()
        {
            this.AppData.BusyCount++;

            await Task.Delay(250); // tiny delay to simulate a real connection
            this.AppData.MessageCount++;
            var craftTelemetry = (CraftTelemetry)PegasusMessage.Decode(FakeCraftTelemetryLine);
            this.CurrentCraftTelemetry.Data = craftTelemetry;
            this.CurrentChaseTelemetry.Data = CreateGroundTelemetry(craftTelemetry, true);
            this.CurrentLaunchTelemetry.Data = CreateGroundTelemetry(craftTelemetry, false);

            this.AppData.BusyCount--;
        }

        public async Task SendUserMessageAsync(string message)
        {
            this.AppData.StatusMessage = "Sending...";
            this.AppData.BusyCount++;

            UserMessage umessage = new UserMessage();
            umessage.Message = message;
#if DEBUG
            //adding ticks to the user message for testing latency (not used in production)
            umessage.Message += "_" + DateTime.UtcNow.Ticks.ToString();
#endif
            umessage.id = Guid.NewGuid().ToString();
            string jsonString = umessage.ToJson();
            byte[] payload = Encoding.UTF8.GetBytes(jsonString);
            CoapRequest request = new CoapRequest(messageId++,
                                                    RequestMessageType.NonConfirmable,
                                                    MethodType.POST,
                                                    new Uri(UserMessageTopicUri),
                                                    MediaType.Json,
                                                    payload);

            byte[] messageBytes = request.Encode();
            try
            {
                await client.SendAsync(messageBytes);
            }
            finally
            {
                this.AppData.BusyCount--;
            }
        }

        private Task SubscribeTopicAsync(string subscribeUri)
        {
            Uri resourceUri = new Uri(subscribeUri);
            CoapRequest request = new CoapRequest(messageId++, RequestMessageType.NonConfirmable, MethodType.POST, resourceUri, MediaType.Json);
            byte[] message = request.Encode();
            return client.SendAsync(message);
        }

        private void client_OnMessage(object sender, byte[] message)
        {
            lastConnectAttemptTime = DateTime.MinValue;
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

        private void client_OnError(object sender, Exception ex)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                // this.AppData.StatusMessage = "Web Socket error: " + ex.Message;
                this.AppData.BusyCount--;
                this.client = null;
                // Always give us two seconds before trying to reconnect, in case
                // the phone is bringing up a connection.  This also solves an
                // animation problem.
                await Task.Delay(2000); 
                await this.ConnectWebSocketAsync();
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
