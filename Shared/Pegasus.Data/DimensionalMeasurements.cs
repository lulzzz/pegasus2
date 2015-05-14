
namespace Pegasus2.Data
{
    using Newtonsoft.Json;
    using System;


#if !(SILVERLIGHT || WINDOWS_PHONE || NETFX_CORE || PORTABLE)
    [Serializable]
#endif
    public abstract class DimensionalMeasurements
    {
        protected DimensionalMeasurements()
        {
        }

        protected DimensionalMeasurements(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        [JsonProperty("x")]
        public virtual double X { get; set; }

        [JsonProperty("y")]
        public virtual double Y { get; set; }

        [JsonProperty("z")]
        public virtual double Z { get; set; }
    }
}
