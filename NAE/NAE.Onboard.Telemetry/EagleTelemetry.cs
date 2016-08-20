//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NAE.Onboard.Telemetry
//{
//    [Serializable]
//    [JsonObject]
//    public class EagleTelemetry
//    {

//        [JsonProperty("timestamp")]
//        public DateTime Timestamp { get; set; }

//        [JsonProperty("aero1Psi")]
//        public double Aero1Psi { get; set; }

//        [JsonProperty("aero2Psi")]
//        public double Aero2Psi { get; set; }

//        [JsonProperty("aero3Psi")]
//        public double Aero3Psi { get; set; }

//        [JsonProperty("aero4Psi")]
//        public double Aero4Psi { get; set; }

//        [JsonProperty("aero5Psi")]
//        public double Aero5Psi { get; set; }

//        [JsonProperty("aero6Psi")]
//        public double Aero6Psi { get; set; }

//        [JsonProperty("aero7Psi")]
//        public double Aero7Psi { get; set; }

//        [JsonProperty("aero8Psi")]
//        public double Aero8Psi { get; set; }

//        [JsonProperty("aero9Psi")]
//        public double Aero9Psi { get; set; }

//        [JsonProperty("aero10Psi")]
//        public double Aero10Psi { get; set; }

//        [JsonProperty("aero11Psi")]
//        public double Aero11Psi { get; set; }

//        [JsonProperty("aero15Psi")]
//        public double Aero15Psi { get; set; }

//        [JsonProperty("aero16Psi")]
//        public double Aero16Psi { get; set; }

//        [JsonProperty("endevcoForeKph")]
//        public double EndevcoForeKph { get; set; }

//        [JsonProperty("endevcoMidKph")]
//        public double EndevcoMidKph { get; set; }

//        [JsonProperty("endevcoAftKph")]
//        public double EndevcoAftKph { get; set; }

//        /// <summary>
//        /// Position of stick between 0 (full Left) and 1 (full right)
//        /// </summary>
//        [JsonProperty("stickPosition")]
//        public double StickPosition { get; set; }


//        [JsonProperty("steeringBoxPositionDegrees")]
//        public double SteeringBoxPositionDegrees { get; set; }

//        [JsonProperty("noseWeightLbf")]
//        public double NoseWeightLbf { get; set; }

//        [JsonProperty("airSpeedKph")]
//        public double AirSpeedKph { get; set; }

//        [JsonProperty("throttlePosition")]
//        public double ThrottlePosition { get; set; }

//        [JsonProperty("leftRearWeightLbf")]
//        public double LeftRearWeightLbf { get; set; }

//        [JsonProperty("rightRearWeightLbf")]
//        public double RightRearWeightLbf { get; set; }

//        [JsonProperty("accelXG")]
//        public double AccelXG { get; set; }

//        [JsonProperty("accelYG")]
//        public double AccelYG { get; set; }

//        [JsonProperty("accelZG")]
//        public double AccelZG { get; set; }

//        [JsonProperty("steerBoxAccelXG")]
//        public double SteerBoxAccelXG { get; set; }

//        [JsonProperty("steerBoxAccelYG")]
//        public double SteerBoxAccelYG { get; set; }

//        [JsonProperty("steerBoxAccelZG")]
//        public double SteerBoxAccelZG { get; set; }
//    }
//}
