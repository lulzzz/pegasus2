using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAE.Data;

namespace NAE.Onboard.Telemetry
{
    public abstract class TelemetryConverter
    {
        private static double Pitot = 0.3103;
        private static double StickMax = 4.2;
        private static double StickMin = 2.9;

        public static EagleTelemetry Convert(EagleRawTelemetry raw, List<double> offsets)
        {


            EagleTelemetry telemetry = new EagleTelemetry()
            {
                Timestamp = raw.Timestamp,
                Aero1Psi = UnitConverter.Convert(raw.Aero1, Pitot, MeasurementType.VoltsPerPsi, offsets[0]),
                Aero2Psi = UnitConverter.Convert(raw.Aero2, Pitot, MeasurementType.VoltsPerPsi, offsets[1]),
                Aero3Psi = UnitConverter.Convert(raw.Aero3, Pitot, MeasurementType.VoltsPerPsi, offsets[2]),
                Aero4Psi = UnitConverter.Convert(raw.Aero4, Pitot, MeasurementType.VoltsPerPsi, offsets[3]),
                Aero5Psi = UnitConverter.Convert(raw.Aero5, Pitot, MeasurementType.VoltsPerPsi, offsets[4]),
                Aero6Psi = UnitConverter.Convert(raw.Aero6, Pitot, MeasurementType.VoltsPerPsi, offsets[5]),
                Aero7Psi = UnitConverter.Convert(raw.Aero7, Pitot, MeasurementType.VoltsPerPsi, offsets[6]),
                Aero8Psi = UnitConverter.Convert(raw.Aero8, Pitot, MeasurementType.VoltsPerPsi, offsets[7]),
                Aero9Psi = UnitConverter.Convert(raw.Aero9, Pitot, MeasurementType.VoltsPerPsi, offsets[8]),
                Aero10Psi = UnitConverter.Convert(raw.Aero10, Pitot, MeasurementType.VoltsPerPsi, offsets[9]),
                Aero11Psi = UnitConverter.Convert(raw.Aero11, Pitot, MeasurementType.VoltsPerPsi, offsets[10]),
                Aero15Psi = UnitConverter.Convert(raw.Aero15, Pitot, MeasurementType.VoltsPerPsi, offsets[11]),
                Aero16Psi = UnitConverter.Convert(raw.Aero16, Pitot, MeasurementType.VoltsPerPsi, offsets[12]),
                EndevcoForeKph = UnitConverter.Convert(raw.EndevcoFore, MeasurementType.Endevco17834),
                EndevcoMidKph = UnitConverter.Convert(raw.EndevcoMid, MeasurementType.Endevco17834),
                EndevcoAftKph = UnitConverter.Convert(raw.EndevcoAft, MeasurementType.Endevco17834),
                StickPosition = UnitConverter.Convert(StickMin, StickMax, raw.StickPosition),
                SteeringBoxPositionDegrees = UnitConverter.Convert(raw.SteeringBoxPosition, MeasurementType.mVPerArcSec),
                NoseWeightLbf = UnitConverter.Convert(raw.WeightNose, 0.28, MeasurementType.mVPerLbf, null),
                AirSpeedKph = UnitConverter.Convert(raw.Pitot, MeasurementType.CurveFit),
                ThrottlePosition = UnitConverter.Convert(raw.ThrottlePosition, 0, MeasurementType.Volts, null),
                LeftRearWeightLbf = UnitConverter.Convert(raw.WeightRearLeft, 0.08, MeasurementType.mVPerLbf, null),
                RightRearWeightLbf = UnitConverter.Convert(raw.WeightRearRight, 0.20, MeasurementType.mVPerLbf, null),
                AccelXG = UnitConverter.Convert(raw.AccelX, 1000, MeasurementType.mVPerGravity, null),
                AccelYG = UnitConverter.Convert(raw.AccelY, 1000, MeasurementType.mVPerGravity, null),
                AccelZG = UnitConverter.Convert(raw.AccelZ, 1000, MeasurementType.mVPerGravity, null),
                SteerBoxAccelXG = UnitConverter.Convert(raw.SteerBoxAccelX, 10, MeasurementType.mVPerGravity, null),
                SteerBoxAccelYG = UnitConverter.Convert(raw.SteerBoxAccelY, 10, MeasurementType.mVPerGravity, null),
                SteerBoxAccelZG = UnitConverter.Convert(raw.SteerBoxAccelZ0, 10, MeasurementType.mVPerGravity, null)
                
            };

            return telemetry;
        }
    }
}
