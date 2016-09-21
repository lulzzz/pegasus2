using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAE.Onboard.Telemetry
{
    public enum MeasurementType
    {
        VoltsPerPsi,
        mVPerLbf,
        mVPerGravity,
        mVPerArcSec,
        Volts,
        CurveFit,
        Endevco17834,
        Endevco17879,
        Endevco17592,
        Endevco17819        
    }

    public static class UnitConverter
    {
        
        public static double DCOffset { get; set; }

        private const double A = 2.352102;
        private const double B = 2.115838;
        private const double C = 2.352102;
        private const double D = 869.8863;
        private const double F = 2.217952;

        public static double Convert(double min, double max, double value)
        {
            double distance = max - min;
            double weight = max - value;
            return weight / distance;
        }

        private static double CurveFit(double y)
        {
            double z =  D * Math.Pow((((B - C) / (y - A)) - 1), F);
            //y = A + (B - C)/(1 + (x/D)^E)
            //double z1 = A + (B - C) / (1 + Math.Pow((y / D), F));
            double y1 = 2.352102 + (2.115838 - 2.352102)/(1 + Math.Pow((y/869.8863),2.217952));
            return y1;
        }

        public static double Convert(double value, MeasurementType type)
        {

            if(type == MeasurementType.Endevco17834)
            {
                //-0.3V with 1.75 mV/psi
                double endevco17834 = ((value + 0.3) * 1000) / 1.75;
                return CurveFit(endevco17834);
            }

            if(type == MeasurementType.Endevco17879)
            {
                //0.8V with 1.9 mV/psi
                double endevco17879 = ((value - 0.8) * 1000) / 1.9;
                return CurveFit(endevco17879);
            }

            if(type == MeasurementType.Endevco17592)
            {
                //-2.5V with 1.84mV/psi
                double endevco17592 = ((value + 2.5) * 1000) / 1.84;
                return CurveFit(endevco17592);
            }

            if(type == MeasurementType.Endevco17819)
            {
                //-4.1V with 1.7mV/psi
                double endevco17819 = ((value + 4.1) * 1000) / 1.7;
                return CurveFit(endevco17819);
            }

            if(type == MeasurementType.CurveFit)
            {
                return CurveFit(value);
            }

            if (type == MeasurementType.mVPerArcSec)
            {
                return (value * 1000) / 60;
            }

            if (type == MeasurementType.mVPerGravity || type == MeasurementType.mVPerLbf)
            {
                return (value * 1000);
            }
            else
            {
                return 0;
            }
        }

        public static double Convert(double value, double constant, MeasurementType type, double? dcOffset)
        {
            if(type == MeasurementType.VoltsPerPsi && dcOffset.HasValue)
            {
                return (value - dcOffset.Value) / constant;
            }
            else if(type == MeasurementType.VoltsPerPsi && !dcOffset.HasValue)
            {
                return (value / constant);
            }
            else if(type == MeasurementType.mVPerLbf)
            {
                return (value * 1000) / constant;
            }
            else if(type == MeasurementType.mVPerGravity)
            {
                return (value * 1000) / constant;
            }
            else
            {
                return 0;
            }
        }



    }
}
