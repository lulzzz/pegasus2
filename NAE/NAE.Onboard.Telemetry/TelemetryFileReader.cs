using NationalInstruments.Tdms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAE.Onboard.Telemetry
{
    public class TelemetryFileReader
    {
        public static TelemetryFileReader Load(string path, int intervalMilliseconds)
        {
            TelemetryFileReader reader = new TelemetryFileReader(path, intervalMilliseconds);

            return reader;
        }

        private string path;
        private int interval;
        private List<Channel> channels;
        protected TelemetryFileReader(string path, int intervalMilliseconds)
        {
            interval = intervalMilliseconds;
            this.path = path;
                        
        }

        public List<double> GetDCOffsets(double seconds)
        {
            List<double> offsets = new List<double>();
            List<EagleRawTelemetry> list = ReadInternal();

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
                if (start == DateTime.MinValue)
                {
                    start = ert.Timestamp;
                }

                if (start.AddSeconds(seconds) > ert.Timestamp)
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

        private List<EagleRawTelemetry> ReadInternal()
        {
            //int index = 0;
            //int checkPoint = 0;
            long incrTicks = Convert.ToInt64((1d / 2048d) * 100 * TimeSpan.TicksPerMillisecond);
            channels = new List<Channel>();

            using (File tdms = new File(this.path))
            {
                tdms.Open();
                foreach (var group in tdms)
                {
                    foreach (var channel in group)
                    {
                        if (!(channel.Name == "OPEN 12" || channel.Name == "OPEN 14"
                            || channel.Name == "OPEN 14"
                            || channel.Name == "OPEN 31"
                            || channel.Name == "OPEN 32"))
                        {
                            channels.Add(channel);
                        }
                    }
                }

                int max = GetMaxDataCount(channels);

                DateTime timestamp = Convert.ToDateTime(channels[0].Properties["wf_start_time"]);

                //long cnt = channels[0].DataCount;
                int index = 0;
                List<IEnumerable<double>> nestedTelemetry = new List<IEnumerable<double>>();
                List<TelemetryValues> valuesList = new List<TelemetryValues>();
                while (index < channels.Count)
                {
                    if (channels[index].Name.ToLower(CultureInfo.InvariantCulture).Contains("open"))
                    {
                        index++;
                        continue;
                    }

                    List<double> values = new List<double>(channels[index].GetData<double>());

                    int i = 0;
                    DateTime time = DateTime.MinValue;
                    List<DateTime> timestamps = new List<DateTime>();
                    values.RemoveRange(max - 1, values.Count - max);

                    while (i < max)
                    {
                        try
                        {
                            time = time == DateTime.MinValue ? Convert.ToDateTime(channels[index].Properties["wf_start_time"]) : time.AddTicks(Convert.ToInt64(Convert.ToDouble(channels[0].Properties["wf_increment"]) * 1000 * TimeSpan.TicksPerMillisecond));
                        }
                        catch (Exception ex)
                        {
                            time = valuesList[valuesList.Count - 1].Timestamps[i];
                            //time = time.AddTicks(Convert.ToInt64((1d / 2048d) * 1000 * TimeSpan.TicksPerMillisecond));
                        }

                        if (time.Day == 1)
                        {
                            break;
                        }

                        timestamps.Add(time);

                        i++;
                    }

                    TelemetryValues tv = new TelemetryValues() { Name = channels[index].Name, Values = values, Timestamps = timestamps };
                    valuesList.Add(tv);

                    index++;
                }

                return RawTelemetryLoader.Load(valuesList);

            }
        }



        public List<EagleRawTelemetry> Read()
        {
            //int index = 0;
            //int checkPoint = 0;
            long incrTicks = Convert.ToInt64((1d / 2048d) * interval * TimeSpan.TicksPerMillisecond);
            channels = new List<Channel>();

            using (File tdms = new File(this.path))
            {
                tdms.Open();
                foreach (var group in tdms)
                {
                    foreach (var channel in group)
                    {
                        if (!(channel.Name == "OPEN 12" || channel.Name == "OPEN 14"
                            || channel.Name == "OPEN 14"
                            || channel.Name == "OPEN 31"
                            || channel.Name == "OPEN 32"))
                        {
                            channels.Add(channel);
                        }
                    }
                }

                int max = GetMaxDataCount(channels);

                DateTime timestamp = Convert.ToDateTime(channels[0].Properties["wf_start_time"]);

                //long cnt = channels[0].DataCount;
                int index = 0;
                List<IEnumerable<double>> nestedTelemetry = new List<IEnumerable<double>>();
                List<TelemetryValues> valuesList = new List<TelemetryValues>();
                while(index < channels.Count)
                {          
                    if(channels[index].Name.ToLower(CultureInfo.InvariantCulture).Contains("open"))
                    {
                        index++;
                        continue;
                    }      
                        
                    List<double> values = new List<double>(channels[index].GetData<double>());
                    
                    int i = 0;
                    DateTime time = DateTime.MinValue;
                    List<DateTime> timestamps = new List<DateTime>();
                    values.RemoveRange(max - 1, values.Count - max);

                    while(i < max)
                    {
                        try
                        {
                            time = time == DateTime.MinValue ? Convert.ToDateTime(channels[index].Properties["wf_start_time"]) : time.AddTicks(Convert.ToInt64(Convert.ToDouble(channels[0].Properties["wf_increment"]) * 1000 * TimeSpan.TicksPerMillisecond));
                        }
                        catch(Exception ex)
                        {
                            time = valuesList[valuesList.Count - 1].Timestamps[i];
                            //time = time.AddTicks(Convert.ToInt64((1d / 2048d) * 1000 * TimeSpan.TicksPerMillisecond));
                        }

                        if(time.Day == 1)
                        {
                            break;
                        }

                        timestamps.Add(time);                        
                                                
                        i++;
                    }
                    
                    TelemetryValues tv = new TelemetryValues() { Name = channels[index].Name, Values = values, Timestamps = timestamps };
                    valuesList.Add(tv);

                    index++;
                }

                return RawTelemetryLoader.Load(valuesList);

            }
        }

        private int GetMaxDataCount(List<Channel> channnels)
        {
            long min = -1;
            int index = 0;
            while(index < channels.Count)
            {
                min = min < 0 ? channels[index].DataCount : min > channels[index].DataCount ? channels[index].DataCount : min;
                index++;
            }

            return Convert.ToInt32(min);

        }

        private int FindCheckPoint()
        {
            return 0;
            //int index = 0;
            //int checkPoint = 0;

            //using (File tsms = new File(this.path))
            //{
            //    tdms.Open();

            //}
        }


    }
}
