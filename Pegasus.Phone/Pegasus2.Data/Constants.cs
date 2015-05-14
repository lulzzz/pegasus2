using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegasus2.Data
{
    public static class Constants
    {
        public static class PayloadTelemetry
        {
            public static readonly string[] FieldNames =  { "source", "timestamp", "atmosphericPressure", "pressureAltitude", "pressureTemp", "humidity", "tempInside",
                                                "tempOutside", "battery1Level", "battery2Level", "balloonReleased", "mainDeployed", "radiationCps", "videoPositionUp",
                                                "_9DofAccelerometer", "_9DofGyroscope", "_9DofMagnetometer", "radioStrength", "gpsLatitude", "gpsLongitude", "gpsAltitude", 
                                                "gpsSpeed", "gpsDirection", "gpsFix", "gpsSatellites", "receptionErrors", "verticalSpeed", "pictureCount", "uvRays",
                                                "ledsActivated", "bpServoOn", "videoServoOn"};
            public static readonly string[] ComplexTypeNames = { "_9DofAccelerometer", "_9DofGyroscope", "_9DofMagnetometer" };
            public static readonly string[] ComplexTypeFieldNames = { "x", "y", "z" };

        }


        public static class MessageIdentifiers
        {
            public static readonly string[] Keys = { "$:", "G:", "P:", "B:", "C:", "V:", "D:", "R:", "U:","N:" };
            //$: Craft telemetry
            //G: Ground telemetry
            //P: Parachute command
            //B: Delivery System command
            //C: Camera command
            //V: Camera notification
            //D: Parachute deployment
            //R: Balloon released
            //U: User message
            //N: Note from craft
        }

        public static class GroundTelemetry
        {
            public static readonly string[] FieldNames = {"source", "timestamp", "temp", "gpsLatitude", "gpsLongitude", "gpsAltitude", 
                                                             "gpsSpeed", "gpsDirection", "gpsFix", "gpsSatellites","azimuth", "elevation",
                                                         "radioStrength", "receptionErrors", "batteryLevel", "groundDistance", "actualDistance", "peerDistance"};
        }
    }
}
