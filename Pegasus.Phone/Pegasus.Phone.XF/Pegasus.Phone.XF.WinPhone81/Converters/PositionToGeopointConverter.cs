using System;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Data;
using Xamarin.Forms.Maps;

namespace Pegasus.Phone.XF.WinPhone81.Converters
{
    public class PositionToGeopointConverter : IValueConverter
    {
        public PositionToGeopointConverter()
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
            return new Geopoint(new BasicGeoposition
            {
                Latitude = position.Latitude,
                Longitude = position.Longitude
            });
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
