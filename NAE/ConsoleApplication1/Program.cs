using NAE.Onboard.Telemetry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAE.Data;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //WriteJsonSample();


            //Config config = new Config();
            //config.AggregateTelemtryUrl = "http://www.foo.org/telemetry/aggregaterun1.json";
            //config.Drone1VideoUrl = "http://www.foo.org/video/drone1run1.mp4";
            //config.Drone2VideoUrl = "http://www.foo.org/video/drone2run1.mp4";
            //config.Location = "Alvord Desert, OR";
            //config.OnboardTelemetryUrl = "http://www.foo.org/telemetry/run1.json";
            //config.OnboardVideoUrl = "http://www.foo.org/video/cockpitrun1.mp4";
            //config.Pilot = "Jessi Combs";
            //config.RunId = "Run1";
            //config.Timestamp = new DateTime(2016, 9, 30, 14, 23, 12);
            
            //string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            //using (StreamWriter writer = new StreamWriter("D:\\config.txt"))
            //{
            //    writer.Write(json);
            //    writer.Flush();
            //    writer.Close();
            //}

            string tdmsFile = "D:\\NAE\\Files\\160423_180017.tdms";
            TelemetryFileReader reader = TelemetryFileReader.Load(tdmsFile, 500);
            List<EagleRawTelemetry> ertList= reader.Read();
            List<double> offsets = ComputeDCOffsets(ertList, 5);
            List<EagleTelemetry> telemetryList = new List<EagleTelemetry>();
            DateTime nextTime = DateTime.MinValue;

            int index = 0;
            double maxX = 0;
            double maxY = 0;
            double maxZ = 0;
            double maxSpeed = 0;

            foreach(EagleRawTelemetry ert in ertList)
            {
                EagleTelemetry telemetry = TelemetryConverter.Convert(ert, offsets);
                
                if(nextTime == DateTime.MinValue)
                {
                    nextTime = telemetry.Timestamp.AddMilliseconds(500);
                }

                if(telemetry.Timestamp > nextTime)
                {
                    telemetryList.Add(telemetry);
                    nextTime = telemetry.Timestamp.AddMilliseconds(500);
                    maxX = telemetry.AccelXG > maxX ? telemetry.AccelXG : maxX;
                    maxY = telemetry.AccelXG > maxY ? telemetry.AccelYG : maxY;
                    maxZ = telemetry.AccelXG > maxZ ? telemetry.AccelZG : maxZ;
                    maxSpeed = telemetry.AccelXG > maxX ? telemetry.AirSpeedKph : maxSpeed;

                }

                
                index++;
                Console.WriteLine(index);
            }

            EagleTelemetry[] et = telemetryList.ToArray();

            string jsonString1 = JsonConvert.SerializeObject(et, Formatting.Indented);

            StreamWriter writer = new StreamWriter("d:\\sample1.json");
            writer.Write(jsonString1);
            writer.Flush();
            writer.Close();            

            Console.ReadKey();

            ConfigManager cm = new ConfigManager();
            StreamReader sr = new StreamReader("d:\\sample1Config.json");
            string c1string = sr.ReadToEnd();
            sr.Close();

            Config c1 = JsonConvert.DeserializeObject<Config>(c1string);

            sr = new StreamReader("d:\\sample2Config.json");
            string c2string = sr.ReadToEnd();
            sr.Close();

            Config c2 = JsonConvert.DeserializeObject<Config>(c2string);

            List<Config> list = new List<Config>();
            list.Add(c1);
            list.Add(c2);
            cm.Write(list);


            //Config config = new Config();
            //config.Aggregates = new AggregateValues() { MaxAccelX = maxX, MaxAccelY = maxY, MaxAccelZ = maxZ, MaxSpeed = maxSpeed };
            //config.Drone1VideoUrl = "drone1URL";
            //config.Drone2VideoUrl = "drone2URL";
            //config.Location = "Alvord Desert, Oregon USA";
            //config.OnboardTelemetryUrl = "onboardTelemetryURL";
            //config.OnboardVideoUrl = "onboardVideoURL";
            //config.Pilot = "SAMPLE PILOT";
            //config.RunId = "SampleRunID2";
            //config.Timestamp = DateTime.Now;

            //string configString = JsonConvert.SerializeObject(config, Formatting.Indented);
            //writer = new StreamWriter("d:\\sample2Config.json");
            //writer.Write(configString);
            //writer.Flush();
            //writer.Close();
        }

        private static List<double> ComputeDCOffsets(List<EagleRawTelemetry> list, int seconds)
        {
            List<double> offsets = new List<double>();
            double aero1 = 0;
            double aero2 = 0;
            double aero3 = 0;
            double aero4 = 0;
            double aero5 = 0;
            double aero6 = 0;
            double aero7 = 0;
            double aero8 = 0;
            double aero9 = 0;
            double aero10 = 0;
            double aero11 = 0;
            double aero15 = 0;
            double aero16 = 0;

            DateTime start = DateTime.MinValue;
            int cnt = 0;

            foreach (EagleRawTelemetry ert in list)
            {
                if(start == DateTime.MinValue)
                {
                    start = ert.Timestamp;
                }
                
                if(start.AddSeconds(seconds) > ert.Timestamp)
                {
                    aero1 += ert.Aero1;
                    aero2 += ert.Aero2;
                    aero3 += ert.Aero3;
                    aero4 += ert.Aero4;
                    aero5 += ert.Aero5;
                    aero6 += ert.Aero6;
                    aero7 += ert.Aero7;
                    aero8 += ert.Aero8;
                    aero9 += ert.Aero9;
                    aero10 += ert.Aero10;
                    aero11 += ert.Aero11;
                    aero15 += ert.Aero15;
                    aero16 += ert.Aero16;
                    cnt++;
                }
                else
                {
                    break;
                }
            }

            offsets.Add(aero1 / cnt);
            offsets.Add(aero2 / cnt);
            offsets.Add(aero3 / cnt);
            offsets.Add(aero4 / cnt);
            offsets.Add(aero5 / cnt);
            offsets.Add(aero6 / cnt);
            offsets.Add(aero7 / cnt);
            offsets.Add(aero8 / cnt);
            offsets.Add(aero9 / cnt);
            offsets.Add(aero10 / cnt);
            offsets.Add(aero11 / cnt);
            offsets.Add(aero15 / cnt);
            offsets.Add(aero16 / cnt);

            return offsets;

        }

        private static void WriteJsonSample()
        {
            EagleTelemetry t = new EagleTelemetry();
            t.AccelXG = 1.2;
            t.AccelYG = 0.93;
            t.AccelZG = 0.97;
            t.AirSpeedKph = 434.23;
            t.Aero1Psi = 1003.32;
            t.Aero2Psi = 1002.23;
            t.Aero3Psi = 1011.11;
            t.Aero4Psi = 982.216;
            t.Aero5Psi = 932.212;
            t.Aero6Psi = 1102.1234;
            t.Aero7Psi = 1203.132;
            t.Aero8Psi = 1302.14;
            t.Aero9Psi = 702.32;
            t.Aero10Psi = 843.21;
            t.Aero11Psi = 1102.79;
            t.Aero15Psi = 1143.113;
            t.Aero16Psi = 1014.95;
            t.EndevcoAftKph = 432.134;
            t.EndevcoForeKph = 436.215;
            t.EndevcoMidKph = 434.74;
            t.LeftRearWeightLbf = 6702.24;
            t.RightRearWeightLbf = 6712.58;
            t.NoseWeightLbf = 8225.67;
            t.SteerBoxAccelXG = 1.13;
            t.SteerBoxAccelYG = 1.03;
            t.SteerBoxAccelZG = 0.923;
            t.SteeringBoxPositionDegrees = 92.132;
            t.StickPosition = 0.44;
            t.ThrottlePosition = 0.96;
            t.Timestamp = DateTime.UtcNow.AddMilliseconds(123.3234345);


            string jsonString = JsonConvert.SerializeObject(t, Formatting.Indented);

            using (StreamWriter writer = new StreamWriter(@"D:\Pegasus Mission GitHub\NAE\ConsoleApplication1\virtualcockpit_telemetry.json"))
            {
                writer.WriteLine(jsonString);
                writer.Flush();
                writer.Close();
            }
        }
    }
}
