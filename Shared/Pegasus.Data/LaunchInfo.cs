namespace Pegasus2.Data
{
    using Newtonsoft.Json;
    using System;

#if !(SILVERLIGHT || WINDOWS_PHONE || NETFX_CORE || PORTABLE)
    [Serializable]
#endif
    [JsonObject]
    public class LaunchInfo
    {
        public LaunchInfo()
            : base()
        {
        }

        [JsonProperty("launchDateTime")]
        public DateTime LaunchDateTime { get; set; }

        [JsonProperty("isLiveTelemetry")]
        public bool IsLiveTelemetry { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        public bool IsTestTelemetry
        {
            get { return !IsLiveTelemetry; }
        }
    }
}
