

namespace Pegasus2.Data
{
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Text;

    [Serializable]
    [JsonObject]
    public class ParachuteCommand : PegasusMessage
    {
        public ParachuteCommand()
        {
        }

      
        private bool? _now;
        private double? _altitude;
        private const string prefix = "P:";
        private string commandString;

        [JsonProperty("deployNow")]
        public bool? DeployNow
        {
            get { return this._now; }
            set
            {
                this._now = value;
                if(!value.HasValue || !value.Value)
                {
                    this.commandString = null;                    
                }
                else 
                {
                    this.commandString = "!";
                }
            }
        }

        [JsonProperty("deployAltitude")]
        public double? DeployAltitude
        {
            get { return this._altitude; }
            set
            {
                this._altitude = value;
                if (value.HasValue && value.Value > 0)
                {
                    this.commandString = value.Value.ToString();
                }
                else
                {
                    this.commandString = null;
                }
            }
        }

        public override MessageType GetMessageType()
        {
            return MessageType.ParachuteCommand;
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
            return JsonConvert.DeserializeObject<ParachuteCommand>(jsonString);
        }
    }
}
