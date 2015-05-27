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
            if (viewModel.CraftTelemetry.Data == null)
            {
                return;
            }

            while (Map.Pins.Count > 0)
            {
                Map.Pins.RemoveAt(Map.Pins.Count - 1);
            }

            Position? groundPosition = null;
            if (viewModel.GroundTelemetry.Data != null)
            {
                groundPosition = new Position(
                        viewModel.GroundTelemetry.Data.GpsLatitude,
                        viewModel.GroundTelemetry.Data.GpsLongitude);

                Map.Pins.Add(new Pin()
                    {
                        Type = PinType.Place,
                        Position = groundPosition.Value,
                        Label = "Ground Location"
                    });
            }

            Position craftPosition = new Position(
                    viewModel.CraftTelemetry.Data.GpsLatitude,
                    viewModel.CraftTelemetry.Data.GpsLongitude);

            Map.Pins.Add(new Pin()
                {
                    Type = PinType.Place,
                    Position = craftPosition,
                    Label = "Current Location"
                });

            Distance? distance = null;

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

