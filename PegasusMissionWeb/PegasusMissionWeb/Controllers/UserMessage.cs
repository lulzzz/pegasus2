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
    public class UserMessage
    {
        public UserMessage()
        {
        }

        private const string prefix = "{U:";

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }


        public static UserMessage FromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<UserMessage>(jsonString);
        }


        public byte[] ToCraftMessage()
        {
            int v = (byte)Encoding.ASCII.GetBytes(prefix + this.Message).Sum(x => (int)x);
            string suffix = v.ToString("X2");
            string message = String.Format("{0}{1},*{2}", prefix, this.Message, suffix);
            return Encoding.UTF8.GetBytes(message);
        }


        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
