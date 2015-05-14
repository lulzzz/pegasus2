using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegasus2.Data
{
    [Serializable]
    [JsonObject]
    public class GroundTelemetry : PegasusMessage
    {       
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("temp")]
        public double Temperature { get; set; }

        [JsonProperty("gpsLatitude")]
        public double GpsLatitude {get;set;}

        [JsonProperty("gpsLongitude")]
        public double GpsLongitude {get;set;}

        [JsonProperty("gpsAltitude")]
        public double GpsAltitude { get; set; }

        [JsonProperty("gpsSpeed")]
        public double GpsSpeed { get; set; }

        [JsonProperty("gpsDirection")]
        public double GpsDirection { get; set; }

        [JsonProperty("gpsFix")]        
        public bool GpsFix {get;set;}

        [JsonProperty("gpsSatellites")]
        public double GpsSatellites { get; set; }

        [JsonProperty("azimuth")]
        public double Azimuth { get; set; } 

        [JsonProperty("elevation")]
        public double Elevation { get; set; }

        [JsonProperty("radioStrength")]
        public double RadioStrength { get; set; }

        [JsonProperty("receptionErrors")]
        public double ReceptionErrors { get; set; }

        [JsonProperty("batteryLevel")]
        public double BatteryLevel { get; set; }

        [JsonProperty("groundDistance")]
        public double GroundDistance { get; set; }

        [JsonProperty("actualDistance")]
        public double ActualDistance { get; set; }

        [JsonProperty("peerDistance")]
        public double PeerDistance { get; set; }

        public override MessageType GetMessageType()
        {
            return MessageType.GroundTelemetry;
        }

        public override byte[] ToCraftMessage()
        {
            throw new NotImplementedException();
        }

        public override PegasusMessage FromCraftMessage(string message)
        {
            string messageString = message.Substring(2, message.Length - 6);
            string[] parts = messageString.Split(new char[] { ',' });

            int index = 0;
            this.Timestamp = Convert.ToDateTime(parts[index++]);
            this.Temperature = Convert.ToDouble(parts[index++]);
            this.GpsLatitude = Convert.ToDouble(parts[index++]);
            this.GpsLongitude = Convert.ToDouble(parts[index++]);
            this.GpsAltitude = Convert.ToDouble(parts[index++]);
            this.GpsSpeed = Convert.ToDouble(parts[index++]);
            this.GpsDirection = Convert.ToDouble(parts[index++]);
            this.GpsFix = parts[index++] == "0" ? false : true;
            this.GpsSatellites = Convert.ToDouble(parts[index++]);
            this.Azimuth = Convert.ToDouble(parts[index++]);
            this.Elevation = Convert.ToDouble(parts[index++]);
            this.RadioStrength = Convert.ToDouble(parts[index++]);
            this.ReceptionErrors = Convert.ToDouble(parts[index++]);
            this.BatteryLevel = Convert.ToDouble(parts[index++]);
            this.GroundDistance = Convert.ToDouble(parts[index++]);
            this.ActualDistance = Convert.ToDouble(parts[index++]);
            this.PeerDistance = Convert.ToDouble(parts[index++]);

            return this;
        }

        public override string ToJson()
        {
            int index = 0;
            JsonBuilder builder = new JsonBuilder();
            
            builder.BuildJsonField(Constants.GroundTelemetry.FieldNames[index++], this.Source.ToString());
            builder.BuildJsonField(Constants.GroundTelemetry.FieldNames[index++], this.Timestamp.ToString());
            builder.BuildJsonField(Constants.GroundTelemetry.FieldNames[index++], this.Temperature.ToString());
            builder.BuildJsonField(Constants.GroundTelemetry.FieldNames[index++], this.GpsLatitude.ToString());
            builder.BuildJsonField(Constants.GroundTelemetry.FieldNames[index++], this.GpsLongitude.ToString());
            builder.BuildJsonField(Constants.GroundTelemetry.FieldNames[index++], this.GpsAltitude.ToString());
            builder.BuildJsonField(Constants.GroundTelemetry.FieldNames[index++], this.GpsSpeed.ToString());
            builder.BuildJsonField(Constants.GroundTelemetry.FieldNames[index++], this.GpsDirection.ToString());
            builder.BuildJsonFieldBool(Constants.GroundTelemetry.FieldNames[index++], this.GpsFix.ToString());
            builder.BuildJsonField(Constants.GroundTelemetry.FieldNames[index++], this.GpsSatellites.ToString());
            builder.BuildJsonField(Constants.GroundTelemetry.FieldNames[index++], this.Azimuth.ToString());
            builder.BuildJsonField(Constants.GroundTelemetry.FieldNames[index++], this.Elevation.ToString());
            builder.BuildJsonField(Constants.GroundTelemetry.FieldNames[index++], this.RadioStrength.ToString());
            builder.BuildJsonField(Constants.GroundTelemetry.FieldNames[index++], this.ReceptionErrors.ToString());
            builder.BuildJsonField(Constants.GroundTelemetry.FieldNames[index++], this.BatteryLevel.ToString());
            builder.BuildJsonField(Constants.GroundTelemetry.FieldNames[index++], this.GroundDistance.ToString());
            builder.BuildJsonField(Constants.GroundTelemetry.FieldNames[index++], this.ActualDistance.ToString());
            builder.BuildJsonField(Constants.GroundTelemetry.FieldNames[index++], this.PeerDistance.ToString());

            return builder.ToString();
        }
    }
}
