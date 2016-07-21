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

        public static UserMessage Load(byte[] message)
        {
            return JsonConvert.DeserializeObject<UserMessage>(Encoding.UTF8.GetString(message));
        }

        public static UserMessage Load(string jsonString)
        {
            return JsonConvert.DeserializeObject<UserMessage>(jsonString);
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }


        
        

    }
}
