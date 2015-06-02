using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Pegasus.Phone.XF.Utilities;
using Pegasus.Phone.XF.ViewModels.Views;
using System;

namespace Pegasus.Phone.XF
{
	public partial class LocationsView : ContentView
	{
        LocationsViewModel viewModel;
        Distance? oldDistance;

        public LocationsView()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (viewModel == this.BindingContext)
            {
                return;
            }

            if (viewModel != null)
            {
                viewModel.CraftTelemetry.PropertyChanged -= TelemetryChanged;
                viewModel.GroundTelemetry.PropertyChanged -= TelemetryChanged;
            }

            viewModel = this.BindingContext as LocationsViewModel;

            if (viewModel != null)
            {
                viewModel.CraftTelemetry.PropertyChanged += TelemetryChanged;
                viewModel.GroundTelemetry.PropertyChanged += TelemetryChanged;
            }
        }

        private void TelemetryChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // TODO: this should really be done completely in the view model!!

            if (viewModel.CraftTelemetry.Data == null)
            {
                return;
            }

            Pin pin;
            int pinIndex = 0;

            // Xamarin's map renderer can't handle you changing the properties of a Pin,
            // whereas replacing pins on Windows causes a flicker.
            if (Device.OS == TargetPlatform.Android || Device.OS == TargetPlatform.iOS)
            {
                while (Map.Pins.Count > pinIndex)
                {
                    Map.Pins.RemoveAt(Map.Pins.Count - 1);
                }
            }

            Position? groundPosition = null;
            if (viewModel.GroundTelemetry.Data != null)
            {
                groundPosition = new Position(
                        viewModel.GroundTelemetry.Data.GpsLatitude,
                        viewModel.GroundTelemetry.Data.GpsLongitude);

                pin = (Map.Pins.Count <= pinIndex) ? new Pin() : Map.Pins[pinIndex];

                pin.Type = PinType.Place;
                pin.Position = groundPosition.Value;
                pin.Color = Color.Red;
                pin.Label = "Ground Location";

                if (Map.Pins.Count <= pinIndex)
                {
                    Map.Pins.Add(pin);
                }
                pinIndex++;
            }

            Position craftPosition = new Position(
                    viewModel.CraftTelemetry.Data.GpsLatitude,
                    viewModel.CraftTelemetry.Data.GpsLongitude);

            pin = (Map.Pins.Count <= pinIndex) ? new Pin() : Map.Pins[pinIndex];

            pin.Type = PinType.Place;
            pin.Position = craftPosition;
            pin.Color = Color.Green;
            pin.Label = "Current Location";

            if (Map.Pins.Count <= pinIndex)
            {
                Map.Pins.Add(pin);
            }
            pinIndex++;

            // Remove any extra pins
            while (Map.Pins.Count > pinIndex)
            {
                Map.Pins.RemoveAt(Map.Pins.Count - 1);
            }

            Distance? oldRadius = null;
            bool userZoomed = false;
            if (this.Map.VisibleRegion != null &&
                (this.Map.VisibleRegion.LatitudeDegrees != 90) // default view on Android
                )
            {
                oldRadius = this.Map.VisibleRegion.Radius;
                userZoomed = this.oldDistance.HasValue && Math.Abs(this.oldDistance.Value.Meters - oldRadius.Value.Meters) > 1;
            }

            Distance? distance = null;
            if (groundPosition.HasValue)
            {
                distance = groundPosition.Value.DistanceFrom(craftPosition);
            }

            // If the user has zoomed, keep that zoom unless we need to zoom out to see
            // both elements.  Otherwise, zoom to 15 miles or farther if necessary.
            MapSpan newSpan;
            if (userZoomed)
            {
                newSpan =
                  (!distance.HasValue || distance.Value.Miles <= oldRadius.Value.Miles) ?
                      new MapSpan(craftPosition, Map.VisibleRegion.LatitudeDegrees, Map.VisibleRegion.LongitudeDegrees) :
                      MapSpan.FromCenterAndRadius(craftPosition, distance.Value);
            }
            else
            {
                if (!distance.HasValue || distance.Value.Miles < 15)
                {
                    distance = Distance.FromMiles(15);
                }

                newSpan = MapSpan.FromCenterAndRadius(craftPosition, distance.Value);

                this.oldDistance = distance;
            }

            Map.MoveToRegion(newSpan);
        }
    }
}

