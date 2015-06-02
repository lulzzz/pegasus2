using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Pegasus.Phone.XF.Utilities;
using Pegasus.Phone.XF.ViewModels.Views;

namespace Pegasus.Phone.XF
{
	public partial class LocationsView : ContentView
	{
        LocationsViewModel viewModel;

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

            Distance? distance = null;
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

            if (groundPosition.HasValue)
            {
                distance = groundPosition.Value.DistanceFrom(craftPosition);
            }

            // Minimum distance view of 15 miles
            if (!distance.HasValue || distance.Value.Miles < 15)
            {
                distance = Distance.FromMiles(15);
            }

            MapSpan span = MapSpan.FromCenterAndRadius(craftPosition, distance.Value);

            Map.MoveToRegion(span);
        }
    }
}

