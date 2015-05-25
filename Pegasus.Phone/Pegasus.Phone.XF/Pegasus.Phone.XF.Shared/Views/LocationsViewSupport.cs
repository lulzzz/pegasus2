using Pegasus.Phone.XF;
using Pegasus.Phone.XF.ViewModels.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

[assembly: Xamarin.Forms.Dependency(typeof(LocationsViewSupport))]
namespace Pegasus.Phone.XF
{
    public class LocationsViewSupport : ILocationsViewSupport
    {
        LocationsViewModel viewModel;
        ContentView sourceView;
        //Map map;
        Xamarin.Forms.Maps.Map map = new Xamarin.Forms.Maps.Map();

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
            View_BindingContextChanged(null, null);
        }

        private void View_BindingContextChanged(object sender, EventArgs e)
        {
            if (sourceView.BindingContext != null)
            {
                viewModel = (LocationsViewModel)sourceView.BindingContext; // Hard-cast to cause an error
                viewModel.CraftTelemetry.PropertyChanged += CraftTelemetryChanged;
            }
        }

        private void CraftTelemetryChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Xamarin.Forms.Maps.Position craftPosition = new Xamarin.Forms.Maps.Position(
                    viewModel.CraftTelemetry.Data.GpsLatitude,
                    viewModel.CraftTelemetry.Data.GpsLongitude);
            Pin pin = new Pin {
                Type = PinType.Place,
                Position = craftPosition,
                Label = "Current Location"
            };
            if (map.Pins.Count > 0)
            {
                map.Pins.RemoveAt(0);
            }
            map.Pins.Add(pin);

            MapSpan span = MapSpan.FromCenterAndRadius(craftPosition,
                Distance.FromMiles(5));

            map.MoveToRegion(span);
        }
    }
}
