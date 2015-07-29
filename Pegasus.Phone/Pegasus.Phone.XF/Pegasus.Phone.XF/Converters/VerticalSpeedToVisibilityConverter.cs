using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pegasus.Phone.XF.Converters
{
    public class VerticalSpeedToVisibilityConverter : IValueConverter
    {
        public bool PositiveIsInvisible { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double? v = value as double?;
            if (!v.HasValue)
            {
                return null;
            }

            double speed = v.Value;

            return (speed >= 0) ^ PositiveIsInvisible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
