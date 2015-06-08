

namespace Pegasus2.Data
{
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;

    [Serializable]
    [JsonObject]
    public class PegasusResponse : PegasusMessage
    {
        public PegasusResponse()
        {
        }

        [JsonProperty("responseType")]
        public ResponseType ResponseMessageType { get; set; }

        public override MessageType GetMessageType()
        {
            return MessageType.CraftResponse;
        }

        public override byte[] ToCraftMessage()
        {
            throw new NotImplementedException();
        }

        public override PegasusMessage FromCraftMessage(string message)
        {
            string messageString = message.Substring(2, message.Length - 6);
            string[] parts = messageString.Split(new char[] { ',' });
            string id = parts[0];

            if(id == "B")
            {
                this.ResponseMessageType = ResponseType.DeliverySystem;
            }
            else if(id == "P")
            {
                this.ResponseMessageType = ResponseType.Parachute;
            }
            else if(id == "U")
            {
                this.ResponseMessageType = ResponseType.UserMessage;
            }
            else
            {
                Trace.TraceWarning("Unexpected response identifier.");
                this.ResponseMessageType = ResponseType.Unknown;
            }

            return this;
        }

        public override string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public override PegasusMessage FromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<PegasusResponse>(jsonString);
        }
    }
}
