using Pegasus.Phone.XF.ViewModels.Views;
using Pegasus.Phone.XF.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

[assembly: Xamarin.Forms.Dependency(typeof(MapsSupport))]
namespace Pegasus.Phone.XF.Windows
{
    public class MapsSupport : IMapsSupport
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
