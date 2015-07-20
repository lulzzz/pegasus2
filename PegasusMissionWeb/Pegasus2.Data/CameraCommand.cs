

namespace Pegasus2.Data
{
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Text;

    [Serializable]
    [JsonObject]
    public class CameraCommand : PegasusMessage
    {
        private const string prefix = "V:";

        [JsonProperty("position")]
        public VideoPosition Position { get; set; }

        
        public static CameraCommand FromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<CameraCommand>(jsonString);
        }

        public override MessageType GetMessageType()
        {
            return MessageType.CameraCommand;
        }

        public override byte[] ToCraftMessage()
        {
            string commandString = this.Position == VideoPosition.Out ? "O" : "U";
            int v = (byte)Encoding.ASCII.GetBytes(prefix + commandString).Sum(x => (int)x);
            string suffix = v.ToString("X2");
            string message = String.Format("{0}{1},*{2}", prefix, commandString, suffix);
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

    }
}
