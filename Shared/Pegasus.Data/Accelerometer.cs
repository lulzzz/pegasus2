

namespace Pegasus2.Data
{
    using Newtonsoft.Json;
    using System;

#if !(SILVERLIGHT || WINDOWS_PHONE || NETFX_CORE || PORTABLE)
    [Serializable]
#endif
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
