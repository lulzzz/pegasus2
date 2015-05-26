using Pegasus.Phone.XF.ViewModels.Views;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Pegasus.Phone.XF
{
	public partial class LocationsView : ContentView
	{
        LocationsViewModel viewModel;

		public LocationsView()
		{
			InitializeComponent();
            BindingContext = viewModel = new LocationsViewModel();

            if (this.Map != null)
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

            Position craftPosition = new Position(
                    viewModel.CraftTelemetry.Data.GpsLatitude,
                    viewModel.CraftTelemetry.Data.GpsLongitude);

            Map.Pins.Add(new Pin()
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

                Map.Pins.Add(new Pin()
                    {
                        Type = PinType.Place,
                        Position = groundPosition,
                        Label = "Ground Location"
                    });
            }

            MapSpan span = MapSpan.FromCenterAndRadius(craftPosition,
                Distance.FromMiles(15));

            Map.MoveToRegion(span);
        }
 
    }
}

