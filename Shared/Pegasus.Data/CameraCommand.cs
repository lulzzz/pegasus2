

namespace Pegasus2.Data
{
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Text;

#if !(SILVERLIGHT || WINDOWS_PHONE || NETFX_CORE || PORTABLE)
    [Serializable]
#endif
    [JsonObject]
    public class CameraCommand : PegasusMessage
    {
        private const string prefix = "V:";

        [JsonProperty("position")]
        public VideoPosition Position { get; set; }


        public override MessageType GetMessageType()
        {
            return MessageType.CameraCommand;
        }

        public override byte[] ToCraftMessage()
        {
            string commandString = this.Position == VideoPosition.Out ? "O" : "U";
            int v = (byte)Encoding.UTF8.GetBytes(prefix + commandString).Sum(x => (int)x);
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

        public override PegasusMessage FromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<CameraCommand>(jsonString);
        }
    }
}
