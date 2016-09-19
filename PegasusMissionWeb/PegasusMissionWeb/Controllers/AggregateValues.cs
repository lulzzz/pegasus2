using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAE.Data
{
    [Serializable]
    [JsonObject]
    public class AggregateValues
    {
        public AggregateValues()
        {
        }

        [JsonProperty("maxSpeed")]
        public double MaxSpeed { get; set; }

        [JsonProperty("maxAccelX")]
        public double MaxAccelX { get; set; }

        [JsonProperty("maxAccelY")]
        public double MaxAccelY { get; set; }

        [JsonProperty("maxAccelZ")]
        public double MaxAccelZ { get; set; }


    }
}
