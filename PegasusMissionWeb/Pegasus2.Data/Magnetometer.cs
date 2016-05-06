

namespace Pegasus2.Data
{
    using Newtonsoft.Json;
    using System;

    [Serializable]
    [JsonObject]
    public class Magnetometer : DimensionalMeasurements
    {
        public Magnetometer()
            : base()
        {
        }

        public Magnetometer(double x, double y, double z)
            : base(x, y, z)
        {
        }

    }
}
