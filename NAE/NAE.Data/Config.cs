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
    public class Config
    {
        public Config()
        {
        }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("runId")]
        public string RunId { get; set; }
        
        [JsonProperty("pilot")]
        public string Pilot { get; set; }                

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("drone1VideoUrl")]
        public string Drone1VideoUrl { get; set; }

        [JsonProperty("drone2VideoUrl")]
        public string Drone2VideoUrl { get; set; }

        [JsonProperty("onboardVideoUrl")]
        public string OnboardVideoUrl { get; set; }

        [JsonProperty("onboardTelemetryUrl")]
        public string OnboardTelemetryUrl { get; set; }

        [JsonProperty("AggregateTelemetryUrl")]
        public string AggregateTelemtryUrl { get; set; }

    }
}
