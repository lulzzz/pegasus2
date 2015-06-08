

namespace Pegasus2.Data
{
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Text;

    [Serializable]
    [JsonObject]
    public class DeliverySystemCommand : PegasusMessage
    {
        public DeliverySystemCommand()
        {
        }

        private bool? _now;
        private TimeSpan? _releaseTime;
        private const string prefix = "B:";
        private string commandString;

        [JsonProperty("releaseNow")]
        public bool? ReleaseNow
        {
            get { return this._now; }
            set
            {
                this._now = value;
                if (!value.HasValue || !value.Value)
                {
                    this.commandString = null;
                }
                else
                {
                    this.commandString = "!";
                }
            }
        }

        [JsonProperty("releaseTime")]
        public TimeSpan? ReleaseTime
        {
            get { return this._releaseTime; }
            set
            {
                this._releaseTime = value;
                if (value.HasValue)
                {
                    this.commandString = value.Value.ToString("hhmm");
                }
                else
                {
                    this.commandString = null;
                }
            }
        }

        public override MessageType GetMessageType()
        {
            return MessageType.DeliverySystemCommand;
        }

        public override byte[] ToCraftMessage()
        {
            int v = (byte)Encoding.ASCII.GetBytes(prefix + this.commandString).Sum(x => (int)x);
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
            return JsonConvert.DeserializeObject<DeliverySystemCommand>(jsonString);
        }
    }
}
