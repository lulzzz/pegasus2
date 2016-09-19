using NationalInstruments.Tdms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NAE.Onboard.Telemetry
{
    [Serializable]
    [JsonObject]
    public class EagleRawTelemetry
    {

        private static EagleRawTelemetry Load(DateTime timestamp, List<Channel> channels)
        {
            EagleRawTelemetry ert = new EagleRawTelemetry();
            return ert;
        }

        public EagleRawTelemetry()
        {
        }


        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("aero1")]
        public double Aero1 { get; set; }

        [JsonProperty("aero2")]
        public double Aero2 { get; set; }

        [JsonProperty("aero3")]
        public double Aero3 { get; set; }

        [JsonProperty("aero4")]
        public double Aero4 { get; set; }

        [JsonProperty("aero5")]
        public double Aero5 { get; set; }

        [JsonProperty("aero6")]
        public double Aero6 { get; set; }

        [JsonProperty("aero7")]
        public double Aero7 { get; set; }

        [JsonProperty("aero8")]
        public double Aero8 { get; set; }

        [JsonProperty("aero9")]
        public double Aero9 { get; set; }

        [JsonProperty("aero10")]
        public double Aero10 { get; set; }

        [JsonProperty("aero11")]
        public double Aero11 { get; set; }

        [JsonProperty("aero15")]
        public double Aero15 { get; set; }

        [JsonProperty("aero16")]
        public double Aero16 { get; set; }

        [JsonProperty("endevcoFore")]
        public double EndevcoFore { get; set; }

        [JsonProperty("endevcoMid")]
        public double EndevcoMid { get; set; }

        [JsonProperty("endevcoAft")]
        public double EndevcoAft { get; set; }

        [JsonProperty("stickPosition")]
        public double StickPosition { get; set; }

        [JsonProperty("steeringBoxPosition")]
        public double SteeringBoxPosition { get; set; }

        [JsonProperty("noseWeight")]
        public double WeightNose { get; set; }

        [JsonProperty("airSpeed")]
        public double Pitot { get; set; }

        [JsonProperty("throttlePosition")]
        public double ThrottlePosition { get; set; }

        [JsonProperty("leftRearWeight")]
        public double WeightRearLeft { get; set; }

        [JsonProperty("rightRearWeight")]
        public double WeightRearRight { get; set; }

        [JsonProperty("accelX")]
        public double AccelX { get; set; }

        [JsonProperty("accelY")]
        public double AccelY { get; set; }

        [JsonProperty("accelZ")]
        public double AccelZ { get; set; }

        [JsonProperty("steerBoxAccelX")]
        public double SteerBoxAccelX { get; set; }

        [JsonProperty("steerBoxAccelY")]
        public double SteerBoxAccelY { get; set; }

        [JsonProperty("steerBoxAccelZ")]
        public double SteerBoxAccelZ0 { get; set; }
    }
}
