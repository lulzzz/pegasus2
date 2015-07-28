using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pegasus.Phone.XF.Converters
{
    public class DegreesToCompassPointConverter : IValueConverter
    {
        static readonly string[] lookups = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double? v = value as double?;
            if (!v.HasValue)
            {
                return null;
            }

            double degrees = v.Value + 22.5;
            if (degrees < 0)
            {
                degrees += 360;
            }
            degrees %= 360;
            degrees /= 45;

            return lookups[(int)degrees];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
