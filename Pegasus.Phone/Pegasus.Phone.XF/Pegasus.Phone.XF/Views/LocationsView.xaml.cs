using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Pegasus.Phone.XF.Utilities;
using Pegasus.Phone.XF.ViewModels.Views;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Pegasus.Phone.XF
{
	public partial class LocationsView : ContentView
	{
        LocationsViewModel viewModel;
        Distance? oldDistance;
        Distance? oldViewRadius;
        bool userZoomed;

        public LocationsView()
        {
            InitializeComponent();
        }

        protected override async void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == "IsVisible" && this.IsVisible)
            {
                // This nastiness is because of a race condition in initializing the map
                await Task.Delay(100);
                TelemetryChanged();
            }
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
                viewModel.ChaseTelemetry.PropertyChanged -= TelemetryChanged;
                viewModel.LaunchTelemetry.PropertyChanged -= TelemetryChanged;
            }

            viewModel = this.BindingContext as LocationsViewModel;

            if (viewModel != null)
            {
                viewModel.CraftTelemetry.PropertyChanged += TelemetryChanged;
                viewModel.ChaseTelemetry.PropertyChanged += TelemetryChanged;
                viewModel.LaunchTelemetry.PropertyChanged += TelemetryChanged;
            }
        }

        private void TelemetryChanged(object sender = null, System.ComponentModel.PropertyChangedEventArgs e = null)
        {
            if (!this.IsVisible)
            {
                return;
            }

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

            Position? chasePosition = null;
            if (viewModel.ChaseTelemetry.Data != null)
            {
                chasePosition = viewModel.ChaseTelemetry.Data.ToPosition();

                pin = (Map.Pins.Count <= pinIndex) ? new Pin() : Map.Pins[pinIndex];

                pin.Type = PinType.Place;
                pin.Position = chasePosition.Value;
                pin.Color = Color.Red;
                pin.Label = "Chase Location";

                if (Map.Pins.Count <= pinIndex)
                {
                    Map.Pins.Add(pin);
                }
                pinIndex++;
            }

            Position? launchPosition = null;
            if (viewModel.LaunchTelemetry.Data != null)
            {
                launchPosition = viewModel.LaunchTelemetry.Data.ToPosition();

                pin = (Map.Pins.Count <= pinIndex) ? new Pin() : Map.Pins[pinIndex];

                pin.Type = PinType.Place;
                pin.Position = launchPosition.Value;
                pin.Color = Color.Purple;
                pin.Label = "Launch Location";

                if (Map.Pins.Count <= pinIndex)
                {
                    Map.Pins.Add(pin);
                }
                pinIndex++;
            }

            Position craftPosition = viewModel.CraftTelemetry.Data.ToPosition();

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

            if (this.Map.VisibleRegion != null &&
                this.Map.VisibleRegion.LatitudeDegrees != 90 &&
                this.Map.VisibleRegion.LongitudeDegrees != 180)
            {
                oldViewRadius = this.Map.VisibleRegion.Radius;
                userZoomed = userZoomed ||
                    (this.oldDistance.HasValue &&
                     Math.Abs(this.oldDistance.Value.Meters - oldViewRadius.Value.Meters) > 10);
            }

            Distance? distance = null;
            if (chasePosition.HasValue)
            {
                distance = chasePosition.Value.DistanceFrom(craftPosition);
            }

            // If the user has zoomed, keep that zoom unless we need to zoom out to see
            // both elements.  Otherwise, zoom to 2 miles or farther if necessary.
            MapSpan newSpan;
            if (userZoomed)
            {
                newSpan =
                  (!distance.HasValue || distance.Value.Miles <= oldViewRadius.Value.Miles) ?
                      new MapSpan(craftPosition, Map.VisibleRegion.LatitudeDegrees, Map.VisibleRegion.LongitudeDegrees) :
                      MapSpan.FromCenterAndRadius(craftPosition, distance.Value);
            }
            else
            {
                if (!distance.HasValue || distance.Value.Miles < 2)
                {
                    distance = Distance.FromMiles(2);
                }

                newSpan = MapSpan.FromCenterAndRadius(craftPosition, distance.Value);

                this.oldDistance = distance;
            }

            Map.MoveToRegion(newSpan);
        }
    }
}

