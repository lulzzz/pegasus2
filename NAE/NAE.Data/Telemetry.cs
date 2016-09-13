using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAE.Data
{
    [Serializable]
    [JsonObject]
    public class Telemetry
    {
        public Telemetry()
        {
        }

        private const string prefix = "$:";

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("runId")]
        public string RunId { get; set; }

        [JsonProperty("gpsLatStart")]
        public double GpsLatitudeStart { get; set; }

        [JsonProperty("gpsLonStart")]
        public double GpsLongitudeStart { get; set; }

        [JsonProperty("gpsLat")]
        public double GpsLatitude { get; set; }

        [JsonProperty("gpsLon")]
        public double GpsLongitude { get; set; }

        [JsonProperty("gpsAltitude")]
        public double GpsAltitude { get; set; }

        [JsonProperty("gpsSpeedKph")]
        public double GpsSpeedKph { get; set; }

        [JsonProperty("gpsSpeedMph")]
        public double GpsSpeedMph { get; set; }

        [JsonProperty("gpsDirection")]
        public double GpsDirection { get; set; }

        [JsonProperty("gpsFix")]
        public bool SatelliteFix { get; set; }

        [JsonProperty("gpsSatelites")]
        public int Satellites { get; set; }

        [JsonProperty("atmTemp")]
        public double Temperature { get; set; }

        [JsonProperty("atmHumidity")]
        public double Humidity { get; set; }

        [JsonProperty("atmPressure")]
        public double Pressure { get; set; }

        [JsonProperty("atmAltitude")]
        public double Altitude { get; set; }

        [JsonProperty("imuLinAccelX")]
        public double LinearAccelX { get; set; }

        [JsonProperty("imuLinAccelY")]
        public double LinearAccelY { get; set; }

        [JsonProperty("imuLinAccelZ")]
        public double LinearAccelZ { get; set; }

        [JsonProperty("imuHeading")]
        public double Yaw { get; set; }

        [JsonProperty("imuPitch")]
        public double Pitch { get; set; }

        [JsonProperty("imuRoll")]
        public double Roll { get; set; }

        [JsonProperty("soundAmp")]
        public double Sound { get; set; }

        [JsonProperty("devVoltage")]
        public double Voltage { get; set; }

        [JsonProperty("devCurrent")]
        public int Current { get; set; }

        public static Telemetry Load(string csvString)
        {
            int checkValue = 0;
            byte checkSum = 0;

            if (!csvString.Contains("*")) //not a valid message; no check value
            {
                return null;
            }

            string messagePrefix = csvString.Substring(0, 2); //identifies message type

            if (messagePrefix != prefix)
            {
                return null;
            }

            string messageString = csvString.Substring(2, csvString.Length - 6);  //the message without identifier and check value
            string checkValueString = csvString.Substring(csvString.Length - 2, 2); //the check value

            string[] parts = messageString.Split(new char[] { ',' }); //the message parts as string array

            //get the check value as an int
            if (!int.TryParse(csvString.Substring(csvString.Length - 2, 2), NumberStyles.AllowHexSpecifier, null, out checkValue))
            {
                return null;
            }

            //compute the check sum
            checkSum = (byte)Encoding.ASCII.GetBytes(csvString.Substring(0, csvString.Length - 4)).Sum(x => (int)x);

            //check value should equal check value; otherwise invalid message 
            if (checkSum != (byte)checkValue)
            {
                return null;
            }

            int index = 0;HebrewCalendar 
            Telemetry instance = new Telemetry();
            instance.Timestamp = Convert.ToDateTime(parts[index++]);
            instance.GpsLatitude = Convert.ToDouble(parts[index++]);
            instance.GpsLongitude = Convert.ToDouble(parts[index++]);
            instance.GpsAltitude = Convert.ToDouble(parts[index++]);
            instance.GpsSpeedKph = Convert.ToDouble(parts[index++]);
            instance.GpsSpeedMph = Convert.ToDouble(parts[index++]);
            instance.GpsDirection = Convert.ToDouble(parts[index++]);
            instance.SatelliteFix = Convert.ToBoolean(Convert.ToInt32((parts[index++])));
            instance.Satellites = Convert.ToInt32(parts[index++]);
            instance.Temperature = Convert.ToDouble(parts[index++]);
            instance.Humidity = Convert.ToDouble(parts[index++]);
            instance.Pressure = Convert.ToDouble(parts[index++]);
            instance.Altitude = Convert.ToDouble(parts[index++]);
            instance.LinearAccelX = Convert.ToDouble(parts[index++]);
            instance.LinearAccelY = Convert.ToDouble(parts[index++]);
            instance.LinearAccelZ = Convert.ToDouble(parts[index++]);
            instance.Yaw = Convert.ToDouble(parts[index++]);
            instance.Pitch = Convert.ToDouble(parts[index++]);
            instance.Roll = Convert.ToDouble(parts[index++]);
            instance.Sound = Convert.ToDouble(parts[index++]);
            instance.Voltage = Convert.ToDouble(parts[index++]);
            instance.Current = Convert.ToInt32(parts[index++]);

            return instance;
        }

    }
}
