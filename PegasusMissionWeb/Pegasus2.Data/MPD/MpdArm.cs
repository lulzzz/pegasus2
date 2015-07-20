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
    public class MpdArm
    {
        [JsonProperty("armed")]
        public bool Armed { get; set; }

        [JsonProperty("pressure")]
        public double Pressure { get; set; }

        public static MpdArm FromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<MpdArm>(jsonString);
        }
    }
}
