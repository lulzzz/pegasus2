using Pegasus.Phone.XF;
using Pegasus.Phone.XF.ViewModels.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

[assembly: Xamarin.Forms.Dependency(typeof(LocationsViewSupport))]
namespace Pegasus.Phone.XF
{
    public class LocationsViewSupport : ILocationsViewSupport
    {
        LocationsViewModel viewModel;
        ContentView sourceView;
        Map map;

        public void BindToView(ContentView view)
        {
            map = new Xamarin.Forms.Maps.Map();

            sourceView = view;
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
                viewModel.GroundTelemetry.PropertyChanged += CraftTelemetryChanged;
            }
        }

        private void CraftTelemetryChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (viewModel.CraftTelemetry.Data == null)
            {
                return;
            }

            while (map.Pins.Count > 0)
            {
                map.Pins.RemoveAt(map.Pins.Count - 1);
            }

            Position craftPosition = new Position(
                    viewModel.CraftTelemetry.Data.GpsLatitude,
                    viewModel.CraftTelemetry.Data.GpsLongitude);

            map.Pins.Add(new Pin()
                {
                    Type = PinType.Place,
                    Position = craftPosition,
                    Label = "Current Location"
                });

            if (viewModel.GroundTelemetry.Data != null)
            {
                Position groundPosition = new Position(
                        viewModel.GroundTelemetry.Data.GpsLatitude,
                        viewModel.GroundTelemetry.Data.GpsLongitude);

                map.Pins.Add(new Pin()
                    {
                        Type = PinType.Place,
                        Position = groundPosition,
                        Label = "Ground Location"
                    });
            }

            MapSpan span = MapSpan.FromCenterAndRadius(craftPosition,
                Distance.FromMiles(15));

            map.MoveToRegion(span);
        }
    }
}
