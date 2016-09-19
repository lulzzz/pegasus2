using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAE.Onboard.Telemetry
{
    public class TelemetryValues
    {
        public TelemetryValues()
        {
            this.Values = new List<double>();
        }

        public string Name { get; set; }

        public string PropertyName
        {
            get { return this.Name.Replace(" ", ""); }
        }

        public List<DateTime> Timestamps { get; set; }

        public List<double> Values { get; set; }
        

    }
}
