using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegasus2.Data.MPD
{
    [Serializable]
    [JsonObject]
    public class MpdTime
    {
        [JsonProperty("timeToCommand")]
        public int TimeToCommand { get; set; }

        [JsonProperty("pressure")]
        public double Pressure { get; set; }

        public static MpdTime FromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<MpdTime>(jsonString);
        }
    }
}
