using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FieldGateway.Telemetry.Generator
{
    class Program
    {
        private static string comPort;
        private static DateTime time;
        private static double gpsLat = 40.276905;
        private static double gpsLon = 118.969136;
        private static string runId;
        static void Main(string[] args)
        {
            //Rewrite();


            string[] switches = GetSwitches(args);
            if(switches == null)
            {
                WriteInstructions();
                Console.ReadKey();
                return;                
            }

            if(switches[0] != null)
            {
                comPort = switches[0];
                runId = switches[1];
            }
            else
            {
                runId = switches[1];
            }

            

            Task task = null;

            if (comPort == null)
            {
                //use a Web socket to send
                string host = "ws://broker.pegasusmission.io/api/connect";
                string subprotocol = "coap.v1";
                string token = GetSecurityToken();
                TelemetryManager wsTelemetry = new TelemetryManager(host, subprotocol, token);
                Console.WriteLine("Starting telemetry");
                task = wsTelemetry.RunAsync(runId);
                Task.WhenAll(task);
            }
            else
            {
                //use the serial connection to send (test Field Gateway)
                SerialConnection connection = new SerialConnection(comPort, 38400, 8, System.IO.Ports.StopBits.One, System.IO.Ports.Parity.None);
                connection.Open();

                TelemetryManager tm = new TelemetryManager(connection);
                Console.WriteLine("Starting telemetry");
                task = tm.RunAsync(runId);
                Task.WhenAll(task);
            }

            Console.WriteLine("press any key to stop...");
            Console.ReadKey();


            //using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.RT_Telemetry_Test)))
            //{

            //    using (StreamReader reader = new StreamReader(stream))
            //    {
            //        while (!reader.EndOfStream)
            //        {
            //            string line = reader.ReadLine();
            //            Thread.Sleep(500);                        
            //        }

            //        reader.Close();
            //    }
            //    stream.Close();
            //}

        }

        private static string GetSecurityToken()
        {
            string issuer = ConfigurationManager.AppSettings["issuer"];
            string audience = ConfigurationManager.AppSettings["audience"];
            string signingKey = ConfigurationManager.AppSettings["signingKey"];

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("http://pegasusmission.io/claims/name", Guid.NewGuid().ToString()));
            claims.Add(new Claim("http://pegasusmission.io/claims/role", "gateway"));
            return JwtSecurityTokenBuilder.Create(issuer, audience, claims, 2000, signingKey);
        }


        private static void Rewrite()
        {
            string prefix = "$:";

            StreamWriter writer = new StreamWriter("D:\\tstream.txt");

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(Properties.Resources.RT_Telemetry_Test)))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    while(!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        int v = (byte)Encoding.ASCII.GetBytes(prefix + line).Sum(x => (int)x);
                        string suffix = v.ToString("X2");
                        string message = String.Format("{0}{1},*{2}", prefix, line, suffix);
                        writer.WriteLine(message);
                        
                    }

                    writer.Flush();
                    writer.Close();
                }
            }

            
        }

        

        private static void WriteInstructions()
        {
            Console.WriteLine("---Instructions---");
            Console.WriteLine("Using Serial Port");
            Console.WriteLine("If you have a virtual serial port and use the Field Gateway you may enter these args");
            Console.WriteLine("c:<ComPortName> r:<RunId>");
            Console.WriteLine("");
            Console.WriteLine("If using Web Socket you may enter these args");
            Console.WriteLine("r:<RunId>");
            Console.WriteLine("------------------");
        }

        private static string[] GetSwitches(string[] args)
        {
            string[] switches = new string[2];
            bool hasSwitch = false;
            foreach(string arg in args)
            {
                string[] parts = arg.Split(new char[] { ':' });
                if(parts.Length != 2)
                {
                    continue;
                }
                else
                {
                    if(parts[0] == "c")
                    {
                        switches[0] = parts[1];
                    }

                    if(parts[0] == "r")
                    {
                        switches[1] = parts[1];
                        hasSwitch = true;
                    }
                }
            }


            if(!hasSwitch)
            {
                return null;
            }
            else
            {
                return switches;
            }


        }

        //private static void LoadTelemetry()
        //{

        //    double gpsLongitude = gpsLon;
        //    double gpsAlt = 2000;

        //    double gpsSpeedMph = 400;
        //    double gpsSpeedKph = gpsSpeedMph / 0.621;
        //    double gpsDirection = 180;
        //    int fix = 1;
        //    int sats = 4;
        //    double temp = 20;
        //    double humidity = 50;
        //    double pressure = 990.34;
        //    double alt = 2013;
        //    double accelX = 1.1;
        //    double accelY = 0.2;
        //    double accelZ = 0.3;
        //    double yaw = 180.34;
        //    double pitch = 0.85;
        //    double roll = 90.76;
        //    double sound = 12.1;
        //    double volts = 7.4;
        //    int current = 1234;

        //    //40.276905, -118.969136
        //    time = DateTime.UtcNow;
        //    int index = 0;
        //    DateTime timestamp = time;
        //    using (StreamWriter writer = new StreamWriter("./output.txt"))
        //    {
        //        while (index < 1000)
        //        {
        //            timestamp = timestamp.AddMilliseconds(500);
        //            string timestampString = timestamp.ToString("yyyy-MM-ddTHH:MM:ss.fffZ");

        //            gpsLongitude = gpsLongitude + 0.00001;
        //            gpsAlt = gpsAlt - 0.01;
        //            gpsSpeedMph = gpsSpeedMph + 0.01;
        //            gpsSpeedKph = gpsSpeedMph / 0.621;
        //            gpsDirection = gpsDirection - 0.01;
        //            if (sats < 10)
        //            {
        //                sats = sats + 1;
        //            }
        //            else
        //            {
        //                sats = sats - 1;
        //            }
        //            temp = temp + 0.01;
        //            humidity = humidity + 0.02;
        //            pressure = pressure - 0.03;
        //            alt = alt - 0.01;
        //            accelX = accelX + 0.01;
        //            accelY = accelY + 0.01;
        //            accelZ = accelZ + 0.01;
        //            yaw = yaw - 0.01;
        //            pitch = pitch + 0.01;
        //            roll = roll + 0.01;
        //            sound = sound + 0.01;
        //            volts = volts - 0.01;
        //            if (current < 1000)
        //            {
        //                current = current + 1;
        //            }
        //            else
        //            {
        //                current = current - 1;
        //            }

        //            string line = String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21}", timestampString, gpsLat, gpsLongitude, gpsAlt, gpsSpeedKph, gpsSpeedMph, gpsDirection, fix, sats, temp, humidity, pressure, alt, accelX, accelY, accelZ, yaw, pitch, roll, sound, volts, current);
        //            writer.WriteLine(line);
        //            //builder.Append(line + "\r\n");


        //            index++;
        //        }

        //        writer.Flush();
        //        writer.Close();
        //    }

        //}




    }
}
