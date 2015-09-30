using Newtonsoft.Json;
using Pegasus2.Data;
using Piraeus.Protocols.Coap;
using Piraeus.Web.WebSockets;
using Piraeus.Web.WebSockets.Net45;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace P2TelemetryGenerator
{
    class Program
    {

        private static int telemetryType;
        private static int delay;
        private static List<string> craftTelemetryList;
        private static string telemetrySource;
        private static double launchLatitude = 46.8301;
        private static double launchLongitude = -119.1643;
        private static double launchAltitude = 198.8;

       
        //private static string host = "ws://localhost:11748/api/connect";
        //private static string host = "wss://broker.pegasusmission.io/api/connect";
        private static string host = "ws://broker.pegasusmission.io/api/connect";
        private static string subprotocol = "coap.v1";
        private static string securityTokenString;
        private static string issuer = "urn:pegasusmission.io";
        private static string audience = "http://broker.pegasusmission.io/api/connect";
        private static string signingKey = "cW0iA3P/mhFi0/O4EAja7UuJ16q6Aeg4cOzL7SIvLL8=";
        private static bool opened;
        private static ushort messageId;
        private static string craftTelemetryTopicUriString = "coaps://pegasusmission.io/publish?topic=http://pegasus2.org/telemetry";
        private static string groundTelemetryTopicUriString = "coaps://pegasusmission.io/publish?topic=http://pegasus2.org/ground";
        private static string userMessageTopicUriString = "coaps://pegasusmission.io/publish?topic=http://pegasus2.org/usermessage";
        private static string userNoteTopic = "coaps://pegasusmission.io/publish?topic=http://pegasus2.org/usernote";
        private static string craftNoteTopic = "coaps://pegasusmission.io/publish?topic=http://pegasus2.org/craftnote";
        private static string dsrCommandTopic = "coaps://pegasusmission.io/publish?topic=http://pegasus2.org/release";
        private static string mpdCommandTopic = "coaps://pegasusmission.io/publish?topic=http://pegasus2.org/deploy";
        private static string videoCommandTopic = "coaps://pegasusmission.io/publish?topic=http://pegasus2.org/video";
        private static string mpdArmedTopic = "coaps://pegasusmission.io/publish?topic=http://pegasus2.org/mpdarmed";
        private static string mpdTimerTopic = "coaps://pegasusmission.io/publish?topic=http://pegasus2.org/mpdtimer";

        private static string role = "gateway";

        private static WebSocketClient_Net45 client;

        static void Main(string[] args)
        {
            Console.WriteLine();
            WriteHeader();
            SelectTelemteryType();

            if(telemetryType <= 4)
            {
                role = "gateway";
                SelectTelemetrySource();
            }
            else if (telemetryType == 5)
            {
                role = "user";
            }
            else if(telemetryType > 5 && telemetryType < 9)
            {
                role = "missioncontrol";
            }
            else
            {
                role = "service";
            }

            LoadCraftTelemetry();
            SetSecurityToken();            
            OpenWebSocket();            

            while(!opened)
            {
                Console.WriteLine("waiting on socket to open...");
                Thread.Sleep(500);
                
            }

            RunMessages();

            Console.WriteLine("Choose another scenario (Y/N) ? ");
            string result = Console.ReadLine().ToLower();
            if (result == "y")
                Main(null);

            Console.WriteLine("Finished...press any key to terminate.");
            Console.ReadKey();
        }

        private static void RunMessages()
        {
            if(telemetryType == 1) //craft only
            {
                SelectDelay();
                RunCraftOnly();
            }
            else if(telemetryType == 2) //craft + one ground source
            {
                //select the ground source
                SelectDelay();
                RunCraftAndOneGround();
            }
            else if (telemetryType == 3) //craft + both gronud sources
            {
                SelectDelay();
                RunCraftAndBothGround();
            }
            else if (telemetryType == 4) //sms broadcast
            {
                RunCraftNote();
            }
            else if (telemetryType == 5) //user message from phone
            {
                RunUserMessage();
            }
            else if (telemetryType == 6) //dsr
            {
                RunDSR();
            }
            else if (telemetryType == 7) //move video
            {
                RunMoveVideoCamera();
            }
            else if (telemetryType == 8) //mpd fom mission control
            {
                RunMpdCommand();
            }
            else if (telemetryType == 9) //trigger MPD from processor
            {
                SelectDelay();
                RunTiggerMpdSequence();
            }
            else
            {
                Console.WriteLine("Confused over your selection");
            }
        }

        private static void RunCraftOnly()
        {
            int index = 0;
            while (index < craftTelemetryList.Count)
            {                
                SendCraftTelemetry(craftTelemetryList[index]);
                Console.WriteLine("Sent message {0}", index);
                if (delay > 0)
                {
                    Thread.Sleep(delay);
                }
                index++;
            }
        }

        private static void RunCraftAndOneGround()
        {
            int index = 0;
            while (index < craftTelemetryList.Count)
            {
                CraftTelemetry ct = JsonConvert.DeserializeObject<CraftTelemetry>(craftTelemetryList[index]);
                GroundTelemetry launchTelemetry = CreateGroundTelemetry(ct, false);
                GroundTelemetry mobileTelemtry = CreateGroundTelemetry(ct, true);                   
                if (telemetrySource == "mobile")
                {
                    SendGroundTelemetry(mobileTelemtry);
                }
                else
                {
                    SendGroundTelemetry(launchTelemetry);
                }

                index++;
                Console.WriteLine("Sent message {0}", index);
                Thread.Sleep(delay);                    
            }
        }

        private static void RunCraftAndBothGround()
        {
            Random ran = new Random();

            int index = 0;
            while (index < craftTelemetryList.Count)
            {
                CraftTelemetry ct = JsonConvert.DeserializeObject<CraftTelemetry>(craftTelemetryList[index]);                
                GroundTelemetry launchTelemetry = CreateGroundTelemetry(ct, false);
                GroundTelemetry mobileTelemtry = CreateGroundTelemetry(ct, true);
                
                SendCraftTelemetry(craftTelemetryList[index]);
                SendGroundTelemetry(launchTelemetry);
                SendGroundTelemetry(mobileTelemtry);
                index++;
                Console.WriteLine("Sent message {0}", index);
                Thread.Sleep(delay);
            }
        }

        private static void RunUserMessage()
        {
            Console.Write("Enter User Message: ? ");
            string result = Console.ReadLine();
            UserMessage um = new UserMessage() { Id = Guid.NewGuid().ToString(), Message = result };
            string jsonString = JsonConvert.SerializeObject(um);
            byte[] payload = Encoding.UTF8.GetBytes(jsonString);

            CoapMessage message = new CoapRequest(messageId++, RequestMessageType.NonConfirmable, MethodType.POST, new Uri(userMessageTopicUriString), MediaType.Json, payload);
            client.SendAsync(message.Encode()).Wait();
        }

        private static void RunCraftNote()
        {
            Console.Write("Enter Craft Note: ? ");
            string result = Console.ReadLine();
            CraftNote note = new CraftNote() { Id = Guid.NewGuid().ToString(), Note = result };
            string jsonString = JsonConvert.SerializeObject(note);
            byte[] payload = Encoding.UTF8.GetBytes(jsonString);
            
            CoapMessage message = new CoapRequest(messageId++, RequestMessageType.NonConfirmable, MethodType.POST, new Uri(craftNoteTopic), MediaType.Json, payload);
            client.SendAsync(message.Encode()).Wait();
        }

        private static void RunDSR()
        {
            DeliverySystemCommand dsc = new DeliverySystemCommand();

            Console.WriteLine("Select DSR Type");
            Console.WriteLine("(1) Now");
            Console.WriteLine("(2) In 'x' minutes");
            Console.Write("Select Option ? ");
            string option = Console.ReadLine();
            if (option == "2")
            {
                Console.WriteLine();
                Console.Write("Enter minutes as TimeSpan ? ");
                string min = Console.ReadLine();
                dsc.ReleaseTime = TimeSpan.Parse(min);
            }
            else
            {
                dsc.ReleaseNow = true;
            }
            
            string jsonString = JsonConvert.SerializeObject(dsc);
            byte[] payload = Encoding.UTF8.GetBytes(jsonString);

            CoapMessage message = new CoapRequest(messageId++, RequestMessageType.NonConfirmable, MethodType.POST, new Uri(dsrCommandTopic), MediaType.Json, payload);
            client.SendAsync(message.Encode()).Wait();
        }

        private static void RunMoveVideoCamera()
        {
            VideoPosition position = VideoPosition.Out;
            Console.WriteLine("Select Video Position");
            Console.WriteLine("(1) Out");
            Console.WriteLine("(2) Up");
            Console.Write("Enter Option ? ");
            string option = Console.ReadLine();
            if(option == "2")
            {
                position = VideoPosition.Up;
            }

            CameraCommand cc = new CameraCommand() { Position = position };
            string jsonString = JsonConvert.SerializeObject(cc);
            byte[] payload = Encoding.UTF8.GetBytes(jsonString);

            CoapMessage message = new CoapRequest(messageId++, RequestMessageType.NonConfirmable, MethodType.POST, new Uri(videoCommandTopic), MediaType.Json, payload);
            client.SendAsync(message.Encode()).Wait();
        }

        private static void RunMpdCommand()
        {
            ParachuteCommand pc = new ParachuteCommand();

            Console.WriteLine("Select MPD Type");
            Console.WriteLine("(1) Now");
            Console.WriteLine("(2) At 'x' altitude");
            Console.Write("Select Option ? ");
            string option = Console.ReadLine();
            if (option == "2")
            {
                Console.WriteLine();
                Console.Write("Enter altitude ? ");
                string alt = Console.ReadLine();
                pc.DeployAltitude = Convert.ToDouble(alt);
            }
            else
            {
                pc.DeployNow = true;
            }

            string jsonString = JsonConvert.SerializeObject(pc);
            byte[] payload = Encoding.UTF8.GetBytes(jsonString);

            CoapMessage message = new CoapRequest(messageId++, RequestMessageType.NonConfirmable, MethodType.POST, new Uri(mpdCommandTopic), MediaType.Json, payload);
            client.SendAsync(message.Encode()).Wait();
        }


        private static void RunTiggerMpdSequence()
        {
            //going to run a shorter and faster sequence of telemetry through to trigger MPD
            //from the processor
            int index = 0;
            while (index < craftTelemetryList.Count)
            {
                Console.WriteLine("Sent message {0}", index);
                CraftTelemetry ct = JsonConvert.DeserializeObject<CraftTelemetry>(craftTelemetryList[index]);
                if (ct.AtmosphericPressure < 30)
                {
                    SendCraftTelemetry(craftTelemetryList[index]);
                    if (delay > 0)
                    {
                        Thread.Sleep(delay);
                    }
                }

                index++;
            }
        }


        //private static void RunUserNote()
        //{
        //    Console.Write("Enter User Note: ? ");
        //    string result = Console.Read();
        //    UserMessage note = new UserMessage() { Id = Guid.NewGuid().ToString(), Message = result };
        //    string jsonString = JsonConvert.SerializeObject(note);
        //    byte[] payload = Encoding.UTF8.GetBytes(jsonString);

        //    CoapMessage message = new CoapRequest(messageId++, RequestMessageType.NonConfirmable, MethodType.POST, new Uri(userNoteTopic), MediaType.Json, payload);
        //    client.SendAsync(message.Encode()).Wait();
        //}


        private static void SendCraftTelemetry(string message)
        {
            //message = message.Replace("02:30", DateTime.UtcNow.Ticks.ToString());
            byte[] payload = Encoding.UTF8.GetBytes(message);
            CoapRequest request = new CoapRequest(messageId++, 
                                                    RequestMessageType.NonConfirmable, 
                                                    MethodType.POST, 
                                                    new Uri(craftTelemetryTopicUriString), 
                                                    MediaType.Json, 
                                                    payload);

            byte[] messageBytes = request.Encode();
            client.SendAsync(messageBytes).Wait();
        }

        private static void SendGroundTelemetry(GroundTelemetry telemetry)
        {
            string jsonString = JsonConvert.SerializeObject(telemetry);
            byte[] payload = Encoding.UTF8.GetBytes(jsonString);
            CoapRequest request = new CoapRequest(messageId++,
                                                    RequestMessageType.NonConfirmable,
                                                    MethodType.POST,
                                                    new Uri(groundTelemetryTopicUriString),
                                                    MediaType.Json,
                                                    payload);

            byte[] messageBytes = request.Encode();
            client.SendAsync(messageBytes).Wait();
        }

        private static void SendUserMessage(string message)
        {
            UserMessage umessage = new UserMessage();
            //adding ticks to the user message for testing latency (not used in production)
            umessage.Message = message + "_" + DateTime.UtcNow.Ticks.ToString();
            umessage.Id = Guid.NewGuid().ToString();
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
        }
        private static GroundTelemetry CreateGroundTelemetry(CraftTelemetry craftTelemetry, bool mobile)
        {
            var r = new Random();
            GroundTelemetry gt = new GroundTelemetry();
            gt.Source = mobile ? "mobile" : "launch";  //telemetrySource;
            gt.Timestamp = craftTelemetry.Timestamp;
            gt.Temperature = r.Next(17, 25);
            
           
            if(mobile)
            {
                //string.Format("{0:000.000;(000.000);zero}", 23.43);  
                gt.GpsAltitude = r.Next(500, 600);
                gt.GpsLatitude = Convert.ToDouble(string.Format("{0:##0.0000}", craftTelemetry.GpsLatitude + r.Next(0, 1210) / 100000.0));
                gt.GpsLongitude = Convert.ToDouble(string.Format("{0:##0.0000}", craftTelemetry.GpsLongitude + r.Next(0, 1210) / 100000.0));
                gt.GpsDirection = r.Next(80, 110);
                gt.GpsSpeed = r.Next(10, 50);                
            }
            else //launch 
            {
                gt.GpsAltitude = launchAltitude;
                gt.GpsLatitude = launchLatitude;
                gt.GpsLongitude = launchLongitude;
                gt.Azimuth = r.Next(45, 120);
                gt.Elevation = r.Next(10, 75);
            }

            gt.GpsFix = true;
            gt.GpsSatellites = r.Next(1, 6);
            gt.RadioStrength = r.Next(50, 98);
            gt.ReceptionErrors = r.Next(0, 2);
            gt.BatteryLevel = r.NextDouble() * 7;

            Location groundLocation = new Location() { Latitude = gt.GpsLatitude, Longitude = gt.GpsLongitude };
            Location craftLocation = new Location() { Latitude = craftTelemetry.GpsLatitude, Longitude = craftTelemetry.GpsLongitude };
            Location fixedLocation = new Location() { Latitude = launchLatitude, Longitude = launchLongitude };

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

        private static void OpenWebSocket()
        {
            if(client != null)
            {
                Task t = Task.Factory.StartNew(async () =>
                    {
                        await client.CloseAsync();
                    });
                
                Task.WhenAll(t);

                while(client.IsConnected)
                {
                    Thread.Sleep(500);
                }
            }

            opened = false;
            client = new WebSocketClient_Net45();
            client.OnClose+=client_OnClose;
            client.OnError+=client_OnError;
            client.OnMessage += client_OnMessage;
            client.OnOpen += client_OnOpen;

            Task task = Task.Factory.StartNew(async () =>
                {
                    await client.ConnectAsync(host, subprotocol, securityTokenString);
                });

            Task.WhenAll(task);

        }

        static void client_OnOpen(object sender, string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Web Socket is open...");
            Console.ResetColor();
            opened = true;
        }

        static void client_OnMessage(object sender, byte[] message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("message received...do something with it.");
            Console.ResetColor();
        }

        static void client_OnError(object sender, Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            if(ex.InnerException != null)
            {
                Console.WriteLine(ex.InnerException.Message);
            }
            Console.ResetColor();
        }

        static void client_OnClose(object sender, string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Web Socket is closed...");
            Console.ResetColor();
        }

        private static void SetSecurityToken()
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("http://pegasusmission.io/claims/name", Guid.NewGuid().ToString()));
            claims.Add(new Claim("http://pegasusmission.io/claims/role", role));
            securityTokenString = JwtSecurityTokenBuilder.Create(issuer, audience, claims, 20, signingKey);
        }

        #region selectors
        static void WriteHeader()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Pegasus II Message Generator");
            Console.WriteLine();
            Console.ResetColor();
        }

        static void SelectTelemteryType()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Select Telemetry Scenario");
            Console.WriteLine("(1) Craft Telemetry only (User)" );
            Console.WriteLine("(2) Ground Telemetry only (User)");
            Console.WriteLine("(3) Craft & Ground Telemetry (User)");
            Console.WriteLine("(4) Craft Note for SMS Broadcast (Gateway)");
            Console.WriteLine("(5) User message from Phone (Gateway)");
            Console.WriteLine("(6) DSR Command (Gateway)");
            Console.WriteLine("(7) Move Video Command (Gateway)");
            Console.WriteLine("(8) Send MPD Command from Mission Control (Gateway)");
            Console.WriteLine("(9) Add Phone for SMS Notification (NM)");
            

            Console.Write("Type selection here ? ");
            string result = Console.ReadLine();

            if(!int.TryParse(result, out telemetryType))
            {                
                Console.WriteLine();
                Console.WriteLine("invalid selection try again");
                SelectTelemteryType();
            }
            else if (telemetryType < 1 || telemetryType > 9)
            {
                Console.WriteLine();
                Console.WriteLine("Not in range try again");
                SelectTelemteryType();
            }

            Console.WriteLine();
            Console.ResetColor();
        }

        static void SelectTelemetrySource()
        {
            int sourceType = 0;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Select Telemetry Source ");
            Console.WriteLine("(1) Mobile");
            Console.WriteLine("(2) Launch");

            string result = Console.ReadLine();

            if (!int.TryParse(result, out sourceType))
            {
                Console.WriteLine();
                Console.WriteLine("invalid selection try again");
                SelectTelemetrySource();
            }
            else if (sourceType < 0 || sourceType > 2)
            {
                Console.WriteLine();
                Console.WriteLine("Not in range try again");
                SelectTelemetrySource();
            }

            telemetrySource = sourceType == 1 ? "mobile" : "launch";

            Console.WriteLine();
            Console.ResetColor();
        }

        static void SelectDelay()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Select Delay between call (ms) ? ");
            string result = Console.ReadLine();

            if(!int.TryParse(result, out delay))
            {
                Console.WriteLine();
                Console.WriteLine("invalid selection try again");
                SelectDelay();
            }
            else if (delay < 0)
            {
                Console.WriteLine();
                Console.WriteLine("Not in range try again");
                SelectDelay();
            }

            Console.WriteLine();
            Console.ResetColor();
        }

        #endregion

        static void LoadCraftTelemetry()
        {
            craftTelemetryList = new List<string>();

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.CraftTelemetry)))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (reader.Peek() != -1)
                    {
                        string line = reader.ReadLine();
                        CraftTelemetry message = (CraftTelemetry)PegasusMessage.Decode(line);
                        //message.Timestamp = DateTime.UtcNow;
                        message.Source = telemetrySource;

                        string jsonString = message.ToJson();
                        craftTelemetryList.Add(jsonString);
                    }
                }
            }                        
        }
    }
}
