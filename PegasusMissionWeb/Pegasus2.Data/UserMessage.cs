

namespace Pegasus2.Data
{
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Text;

    [Serializable]
    [JsonObject]
    public class UserMessage : PegasusMessage
    {
        public UserMessage()
        {

        }

        private const string prefix = "U:";

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        public override MessageType GetMessageType()
        {
            return MessageType.UserMessage;
        }

        public override byte[] ToCraftMessage()
        {            
            int v = (byte)Encoding.ASCII.GetBytes(prefix + this.Message).Sum(x => (int)x);
            string suffix = v.ToString("X2");
            string message = String.Format("{0}{1},*{2}", prefix, this.Message, suffix);
            return Encoding.UTF8.GetBytes(message);
        }

        public override PegasusMessage FromCraftMessage(string message)
        {
            throw new NotImplementedException();
        }

        public override string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public override PegasusMessage FromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<UserMessage>(jsonString);
        }
    }
}
