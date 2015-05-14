

namespace Pegasus2.Data
{
    using Newtonsoft.Json;
    using System;

    [Serializable]
    [JsonObject]
    public class FlightParameters
    {
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("altitude")]
        public double Altitude { get; set; }
    }
}
