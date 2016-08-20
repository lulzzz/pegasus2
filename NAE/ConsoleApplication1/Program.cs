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

            string tdmsFile = "D:\\NAE\\Files\\160423_180017.tdms";
            TelemetryFileReader reader = TelemetryFileReader.Load(tdmsFile, 500);
            List<EagleRawTelemetry> ertList= reader.Read();
            List<double> offsets = ComputeDCOffsets(ertList, 5);
            List<EagleTelemetry> telemetryList = new List<EagleTelemetry>();

            foreach(EagleRawTelemetry ert in ertList)
            {
                EagleTelemetry telemetry = TelemetryConverter.Convert(ert, offsets);
                telemetryList.Add(telemetry);
            }

            Console.ReadKey();
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
