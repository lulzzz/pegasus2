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

        //2015-01-28T23:21:28Z start time

        private static int telemetryType;
        private static int delay;
        private static List<string> craftTelemetryList;
        private static string telemetrySource;
        private static double launchLatitude = 46.8301;
        private static double launchLongitude = -119.1643;
        private static double launchAltitude = 198.8;

        private static string host = "wss://broker.pegasusmission.io/api/connect";
        //private static string host = "ws://localhost:11748/api/connect";
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

        private static string role = "gateway";

        private static IWebSocketClient client;

        static void Main(string[] args)
        {
            WriteHeader();
            SelectTelemteryType();

            if(telemetryType == 4)
            {
                role = "user";
            }
            else
            {
                SelectTelemetrySource();
            }           
            
            SelectDelay();

            LoadCraftTelemetry();
            
            SetSecurityToken();

            OpenWebSocket();

            while(!opened)
            {
                Console.WriteLine("waiting on socket to open...");
                Thread.Sleep(500);
                
            }

            int index = 0;
            if(telemetryType == 1)
            {
                while(index < craftTelemetryList.Count)
                {   
                    SendCraftTelemetry(craftTelemetryList[index]);
                    index++;
                    Console.WriteLine("Sent message {0}", index);
                    Thread.Sleep(delay);                    
                }
            }
            else if (telemetryType == 2)
            {
                while (index < craftTelemetryList.Count)
                {
                    CraftTelemetry ct = JsonConvert.DeserializeObject<CraftTelemetry>(craftTelemetryList[index]);
                    //CraftTelemetry ct = PegasusMessage.Decode(craftTelemetryList[index]) as CraftTelemetry;
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
            else if (telemetryType == 3)
            {
                while (index < craftTelemetryList.Count)
                {
                    CraftTelemetry ct = JsonConvert.DeserializeObject<CraftTelemetry>(craftTelemetryList[index]);
                    //CraftTelemetry ct = PegasusMessage.Decode(craftTelemetryList[index]) as CraftTelemetry;
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
            else
            {
                while(index < 100)
                {
                    string userMessage = String.Format("Message {0}", index);
                    SendUserMessage(userMessage);
                    index++;
                    Console.WriteLine("Sent message {0}", index);
                    Thread.Sleep(delay);
                    
                }
            }


            Console.WriteLine("Finished sending telemetry...press any key to terminate.");
            Console.ReadKey();

        }

        private static void SendCraftTelemetry(string message)
        {            
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
            string jsonString = CraftTelemetry.Decode(message).ToJson();
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
            GroundTelemetry gt = new GroundTelemetry();
            gt.Source = mobile ? "mobile" : "launch";  //telemetrySource;
            gt.Timestamp = craftTelemetry.Timestamp;
            gt.Temperature = new Random().Next(17, 25);
            
            
            if(mobile)
            {
                gt.GpsAltitude = new Random().Next(500, 600);
                gt.GpsLatitude = craftTelemetry.GpsLatitude + .0111;
                gt.GpsLongitude = craftTelemetry.GpsLongitude + 0.0111;
                gt.GpsDirection = new Random().Next(80, 110);
                gt.GpsSpeed = new Random().Next(10, 50);                
            }
            else //launch 
            {
                gt.GpsAltitude = launchAltitude;
                gt.GpsLatitude = launchLatitude;
                gt.GpsLongitude = launchLongitude;
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
            claims.Add(new Claim("http://pegasusmission.io/claims/name", "matt"));
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
            Console.WriteLine("(1) Craft Telemetry only");
            Console.WriteLine("(2) Ground Telemetry only");
            Console.WriteLine("(3) Craft & Ground Telemetry");
            Console.WriteLine("(4) User message from Phone");
            Console.Write("Type selection here ? ");
            string result = Console.ReadLine();

            if(!int.TryParse(result, out telemetryType))
            {                
                Console.WriteLine();
                Console.WriteLine("invalid selection try again");
                SelectTelemteryType();
            }
            else if (telemetryType < 1 || telemetryType > 4)
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
                        message.Source = telemetrySource;

                        string jsonString = message.ToJson();
                        craftTelemetryList.Add(jsonString);
                    }
                }
            }                        
        }
    }
}
