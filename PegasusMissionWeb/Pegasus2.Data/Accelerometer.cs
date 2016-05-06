

namespace Pegasus2.Data
{
    using Newtonsoft.Json;
    using System;

    [Serializable]
    [JsonObject]
    public class Accelerometer : DimensionalMeasurements
    {
        public Accelerometer()
            : base()
        {
        }

        public Accelerometer(double x, double y, double z)
            : base(x, y, z)
        {
        }
    }
}
