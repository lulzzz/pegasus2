
namespace Pegasus2.Data
{
    using Newtonsoft.Json;
    using System;

#if !(SILVERLIGHT || WINDOWS_PHONE || NETFX_CORE || PORTABLE)
    [Serializable]
#endif
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
