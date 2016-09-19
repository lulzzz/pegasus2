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

        public List<EagleRawTelemetry> Read()
        {
            //int index = 0;
            //int checkPoint = 0;
            long incrTicks = Convert.ToInt64((1d / 2048d) * 1000 * TimeSpan.TicksPerMillisecond);
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
