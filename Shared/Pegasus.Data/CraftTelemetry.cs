

namespace Pegasus2.Data
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

#if !(SILVERLIGHT || WINDOWS_PHONE || NETFX_CORE || PORTABLE)
    [Serializable]
#endif
    [JsonObject]
    public class CraftTelemetry : PegasusMessage
    {

        public CraftTelemetry()
        {

        }

        #region JSON Properties
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("atmosphericPressure")]
        public double AtmosphericPressure { get; set; }

        [JsonProperty("pressureAltitude")]
        public double PressureAltitude { get; set; }

        [JsonProperty("pressureTemp")]
        public double PressureTemp { get; set; }

        [JsonProperty("humidity")]
        public double Humidity { get; set; }

        [JsonProperty("tempInside")]
        public double TempInside { get; set; }

        [JsonProperty("tempOutside")]
        public double TempOutside { get; set; }

        [JsonProperty("battery1Level")]
        public double Battery1Level { get; set; }

        [JsonProperty("battery2Level")]
        public double Battery2Level { get; set; }

        [JsonProperty("balloonReleased")]
        public bool BalloonReleased { get; set; }

        [JsonProperty("mainDeployed")]
        public bool MainDeployed { get; set; }

        [JsonProperty("radiationCps")]
        public double RadiationCps { get; set; }

        [JsonProperty("videoPositionUp")]
        public bool VideoPositionUp { get; set; }

        [JsonProperty("_9DofAccelerometer")]
        public Accelerometer Accelerometer { get; set; }

        [JsonProperty("_9DofGyroscope")]
        public Gyroscope Gyroscope { get; set; }

        [JsonProperty("_9DofMagnetometer")]
        public Magnetometer Magnetometer { get; set; }

        [JsonProperty("radioStrength")]
        public double RadioStrength { get; set; }

        [JsonProperty("gpsLatitude")]
        public double GpsLatitude { get; set; }

        [JsonProperty("gpsLongitude")]
        public double GpsLongitude { get; set; }

        [JsonProperty("gpsAltitude")]
        public double GpsAltitude { get; set; }

        [JsonProperty("gpsSpeed")]
        public double GpsSpeed { get; set; }

        [JsonProperty("gpsDirection")]
        public double GpsDirection { get; set; }

        [JsonProperty("gpsFix")]
        public bool GpsFix { get; set; }

        [JsonProperty("gpsSatellites")]
        public double GpsSatellites { get; set; }
                
        [JsonProperty("receptionErrors")]
        public double ReceptionErrors { get; set; }

        [JsonProperty("verticalSpeed")]
        public double VerticalSpeed { get; set; }

        [JsonProperty("pictureCount")]
        public double PictureCount { get; set; }

        [JsonProperty("uvRays")]
        public double UVRays { get; set; }

        [JsonProperty("ledsActivated")]
        public bool LedsActivated { get; set; }
        
        [JsonProperty("bpServoOn")]
        public bool BPServoOn { get; set; }

        [JsonProperty("videoServoOn")]
        public bool VideoServoOn { get; set; }

        [JsonProperty("deploymentAltitude")]
        public double DeploymentAltitude { get; set; }

        [JsonProperty("releaseTime")]
        public DateTime ReleaseTime { get; set; }

        #endregion

       
        public override MessageType GetMessageType()
        {
            return MessageType.CraftTelemetry;
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
            this.AtmosphericPressure = Convert.ToDouble(parts[index++]);
            this.PressureAltitude = Convert.ToDouble(parts[index++]);
            this.PressureTemp = Convert.ToDouble(parts[index++]);
            this.Humidity = Convert.ToDouble(parts[index++]);
            this.TempInside = Convert.ToDouble(parts[index++]);
            this.TempOutside = Convert.ToDouble(parts[index++]);
            this.Battery1Level = Convert.ToDouble(parts[index++]);
            this.Battery2Level = Convert.ToDouble(parts[index++]);
            this.BalloonReleased = parts[index++] == "0" ? false : true;
            this.MainDeployed = parts[index++] == "0" ? false : true;
            this.RadiationCps = Convert.ToDouble(parts[index++]);
            this.VideoPositionUp = parts[index++] == "0" ? false : true;
            this.Accelerometer = new Accelerometer() { X = Convert.ToDouble(parts[index++]), Y = Convert.ToDouble(parts[index++]), Z = Convert.ToDouble(parts[index++]) };
            this.Gyroscope = new Gyroscope() { X = Convert.ToDouble(parts[index++]), Y = Convert.ToDouble(parts[index++]), Z = Convert.ToDouble(parts[index++]) };
            this.Magnetometer = new Magnetometer() { X = Convert.ToDouble(parts[index++]), Y = Convert.ToDouble(parts[index++]), Z = Convert.ToDouble(parts[index++]) };
            this.RadioStrength = Convert.ToDouble(parts[index++]);
            this.GpsLatitude = Convert.ToDouble(parts[index++]);
            this.GpsLongitude = Convert.ToDouble(parts[index++]);
            this.GpsAltitude = Convert.ToDouble(parts[index++]);
            this.GpsSpeed = Convert.ToDouble(parts[index++]);
            this.GpsDirection = Convert.ToDouble(parts[index++]);
            this.GpsFix = parts[index++] == "0" ? false : true;
            this.GpsSatellites = Convert.ToDouble(parts[index++]);
            this.ReceptionErrors = Convert.ToDouble(parts[index++]);
            this.VerticalSpeed = Convert.ToDouble(parts[index++]);
            this.PictureCount = Convert.ToDouble(parts[index++]);
            this.UVRays = Convert.ToDouble(parts[index++]);
            this.LedsActivated = parts[index++] == "0" ? false : true;
            this.BPServoOn = parts[index++] == "0" ? false : true;
            this.VideoServoOn = parts[index++] == "0" ? false : true;
            this.DeploymentAltitude = Convert.ToDouble(parts[index++]);
            this.ReleaseTime = Convert.ToDateTime(parts[index++]);

            return this;

        }

        public override string ToJson()
        {
            JsonBuilder builder = new JsonBuilder();
            int index = 0;
            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.Source.ToString());
            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.Timestamp.ToString());
            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.AtmosphericPressure.ToString());
            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.PressureAltitude.ToString());

            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.PressureTemp.ToString());
            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.Humidity.ToString());
            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.TempInside.ToString());
            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.TempOutside.ToString());
            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.Battery1Level.ToString());
            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.Battery2Level.ToString());
            builder.BuildJsonFieldBool(Constants.PayloadTelemetry.FieldNames[index++], this.BalloonReleased.ToString());
            builder.BuildJsonFieldBool(Constants.PayloadTelemetry.FieldNames[index++], this.MainDeployed.ToString());
            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.RadiationCps.ToString());
            builder.BuildJsonFieldBool(Constants.PayloadTelemetry.FieldNames[index++], this.VideoPositionUp.ToString());

            builder.BuildJsonComplexType(Constants.PayloadTelemetry.FieldNames[index++], 
                Constants.PayloadTelemetry.ComplexTypeFieldNames, 
                new string[] { this.Accelerometer.X.ToString(), this.Accelerometer.Y.ToString(), this.Accelerometer.Z.ToString() });

            builder.BuildJsonComplexType(Constants.PayloadTelemetry.FieldNames[index++],
                Constants.PayloadTelemetry.ComplexTypeFieldNames,
                new string[] { this.Gyroscope.X.ToString(), this.Gyroscope.Y.ToString(), this.Gyroscope.Z.ToString() });

            builder.BuildJsonComplexType(Constants.PayloadTelemetry.FieldNames[index++],
                Constants.PayloadTelemetry.ComplexTypeFieldNames,
                new string[] { this.Magnetometer.X.ToString(), this.Magnetometer.Y.ToString(), this.Magnetometer.Z.ToString() });

            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.RadioStrength.ToString());
            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.GpsLatitude.ToString());
            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.GpsLongitude.ToString());
            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.GpsAltitude.ToString());
            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.GpsSpeed.ToString());
            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.GpsDirection.ToString());
            builder.BuildJsonFieldBool(Constants.PayloadTelemetry.FieldNames[index++], this.GpsFix.ToString());
            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.GpsSatellites.ToString());
            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.ReceptionErrors.ToString());
            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.VerticalSpeed.ToString());
            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.PictureCount.ToString());
            builder.BuildJsonField(Constants.PayloadTelemetry.FieldNames[index++], this.UVRays.ToString());
            builder.BuildJsonFieldBool(Constants.PayloadTelemetry.FieldNames[index++], this.LedsActivated.ToString());
            builder.BuildJsonFieldBool(Constants.PayloadTelemetry.FieldNames[index++], this.BPServoOn.ToString());
            builder.BuildJsonFieldBool(Constants.PayloadTelemetry.FieldNames[index++], this.VideoServoOn.ToString());
            builder.BuildJsonFieldBool(Constants.PayloadTelemetry.FieldNames[index++], this.DeploymentAltitude.ToString());
            builder.BuildJsonFieldBool(Constants.PayloadTelemetry.FieldNames[index++], this.ReleaseTime.ToString());



            return builder.ToString();


        }
    }

}
