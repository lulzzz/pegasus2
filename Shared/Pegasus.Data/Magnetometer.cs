

namespace Pegasus2.Data
{
    using Newtonsoft.Json;
    using System;

#if !(SILVERLIGHT || WINDOWS_PHONE || NETFX_CORE || PORTABLE)
    [Serializable]
#endif
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
