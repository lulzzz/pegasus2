using Pegasus.Phone.XF;
using Pegasus.Phone.XF.ViewModels.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

[assembly: Xamarin.Forms.Dependency(typeof(CraftTelemetryViewSupport))]
namespace Pegasus.Phone.XF
{
    public class CraftTelemetryViewSupport : ICraftTelemetryViewSupport
    {
        CraftTelemetryViewModel craftTelemetry;
        ContentView sourceView;
        Map map;

        public void BindToView(ContentView view)
        {
            sourceView = view;
            sourceView.WidthRequest = 300;
            sourceView.HeightRequest = 300;
            map = new Xamarin.Forms.Maps.Map();
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
            Position craftPosition = new Position(craftTelemetry.Data.GpsLatitude, craftTelemetry.Data.GpsLongitude);
            MapSpan span = MapSpan.FromCenterAndRadius(craftPosition, Distance.FromMiles(5));
            map.MoveToRegion(span);
        }
    }
}
