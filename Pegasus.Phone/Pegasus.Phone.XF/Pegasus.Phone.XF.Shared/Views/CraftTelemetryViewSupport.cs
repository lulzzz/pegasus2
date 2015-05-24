using Pegasus.Phone.XF;
using Pegasus.Phone.XF.CustomControl;
using Pegasus.Phone.XF.ViewModels.Views;
using System;
using Xamarin.Forms;
#if __ANDROID__
using Xamarin.Forms.Maps;
#endif

[assembly: Xamarin.Forms.Dependency(typeof(CraftTelemetryViewSupport))]
namespace Pegasus.Phone.XF
{
    public class CraftTelemetryViewSupport : ICraftTelemetryViewSupport
    {
        CraftTelemetryViewModel craftTelemetry;
        ContentView sourceView;
        //Map map;
#if __ANDROID__
        Xamarin.Forms.Maps.Map map = new Xamarin.Forms.Maps.Map();
#else
        // Windows store apps and windows phone apps
        PegasusMap map = new Pegasus.Phone.XF.CustomControl.PegasusMap();   
#endif

        public void BindToView(ContentView view)
        {
            sourceView = view;
            // using a bigger frame such as 800x800 for windows store app
            sourceView.WidthRequest = 300;
            sourceView.HeightRequest = 300;
            //map = new Xamarin.Forms.Maps.Map();
            //map = new PegasusMap();
            sourceView.Content = map;

            sourceView.BindingContextChanged += View_BindingContextChanged;           
        }

        private void View_BindingContextChanged(object sender, EventArgs e)
        {
            if (sourceView.BindingContext != null)
            {
                craftTelemetry = (CraftTelemetryViewModel)sourceView.BindingContext; // Hard-cast to cause an error
                craftTelemetry.PropertyChanged += TelemetryChanged;
            }
        }

        private void TelemetryChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
#if __ANDROID__
            Xamarin.Forms.Maps.Position craftPosition = new Xamarin.Forms.Maps.Position(
                    craftTelemetry.Data.GpsLatitude,
                    craftTelemetry.Data.GpsLongitude);
            Pin pin = new Pin {
                Type = PinType.Place,
                Position = craftPosition,
                Label = "Current Location"
            };
            if (map.Pins.Count > 0)
                map.Pins.RemoveAt(0);
            map.Pins.Add(pin);
            MapSpan span = MapSpan.FromCenterAndRadius(craftPosition,
                Distance.FromMiles(5));
            map.MoveToRegion(span);
#else
            Pegasus.Phone.XF.CustomControl.Position craftPosition = new Pegasus.Phone.XF.CustomControl.Position();
            craftPosition.Latitude = craftTelemetry.Data.GpsLatitude;
            craftPosition.Longitude = craftTelemetry.Data.GpsLongitude;
            map.CenterPosition = craftPosition;         
#endif
        }
    }
}
