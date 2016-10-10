﻿
namespace Pegasus2.Data
{
    using Newtonsoft.Json;
    using System;

#if !(SILVERLIGHT || WINDOWS_PHONE || NETFX_CORE || PORTABLE)
    [Serializable]
#endif
    [JsonObject]
    public class CraftNote : PegasusMessage
    {
        public CraftNote()
        {
        }

        private const string prefix = "N:";

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        public override MessageType GetMessageType()
        {
            return MessageType.CraftNote;
        }

        public override byte[] ToCraftMessage()
        {
            throw new NotImplementedException();
        }

        public override PegasusMessage FromCraftMessage(string message)
        {
            string messageString = message.Substring(2, message.Length - 6);
            string[] parts = messageString.Split(new char[] { ',' });
            this.Note = parts[0];
            return this;
        }

        public override string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public override PegasusMessage FromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<CraftNote>(jsonString);
        }
    }
}