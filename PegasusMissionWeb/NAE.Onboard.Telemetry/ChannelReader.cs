using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAE.Onboard.Telemetry
{
    public class ChannelReader
    {
        private Dictionary<string, List<double>> container;
        private Dictionary<string, string> map;
         
        public void AddChannel(string name, IEnumerable<double> values)
        {
            List<double> list = new List<double>(values);
            container.Add(name, list);
        }

        public void AddMapSetting(string channelName, string telemetryName)
        {
            map.Add(channelName, telemetryName);
        }

        public List<EagleRawTelemetry> Read()
        {
            return null;
        }



    }
}
