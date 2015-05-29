using System;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Data;
using Xamarin.Forms.Maps;
using Bing.Maps;

namespace Pegasus.Phone.XF.Windows.Converters
{
    public class PositionToLocationConverter : IValueConverter
    {
        public PositionToLocationConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Position? nullablePosition = value as Position?;
            if (!nullablePosition.HasValue)
            {
                return null;
            }

            Position position = nullablePosition.Value;
            return new Location
            {
                Latitude = position.Latitude,
                Longitude = position.Longitude
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
