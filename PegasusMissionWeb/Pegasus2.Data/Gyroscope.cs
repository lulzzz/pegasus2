
namespace Pegasus2.Data
{
    using Newtonsoft.Json;
    using System;

    [Serializable]
    [JsonObject]
    public class Gyroscope : DimensionalMeasurements
    {
        public Gyroscope()
            : base()
        {
        }

        public Gyroscope(double x, double y, double z)
            : base(x, y, z)
        {
        }

    }
}
