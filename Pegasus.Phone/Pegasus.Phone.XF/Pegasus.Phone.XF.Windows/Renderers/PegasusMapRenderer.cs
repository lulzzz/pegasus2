using Xamarin.Forms.Platform.WinRT;
using Bing.Maps;
using BingMap = Bing.Maps.Map;
using Pegasus.Phone.XF.Windows.Renderers;
using Xamarin.Forms.Maps;
using XFMap = Xamarin.Forms.Maps.Map;

// https://visualstudiogallery.msdn.microsoft.com/224eb93a-ebc4-46ba-9be7-90ee777ad9e1

[assembly: ExportRenderer(typeof(XFMap), typeof(PegasusMapRenderer))]
namespace Pegasus.Phone.XF.Windows.Renderers
{
    public class PegasusMapRenderer : ViewRenderer<XFMap, BingMap>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<XFMap> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
            {
                return;
            }

            var map = new BingMap();
            map.Credentials = "Ar63TjGidMOY96jRx8kLubJjOyqKWOI_S3cToA3P0XO9_mQdQEyIowxChrtD9Eii";
            SetNativeControl(map);

            Xamarin.Forms.MessagingCenter.Subscribe<XFMap, MapSpan>(
                this, "MapMoveToRegion", OnMoveToRegionMessage, Element);
        }

        private void OnMoveToRegionMessage(XFMap map, MapSpan span)
        {
            if (map == null || span == null)
            {
                return;
            }

            Pushpin pushpin = new Pushpin();
            pushpin.Text = "Current Location";

            Location currentLocation = new Location(
                span.Center.Latitude,
                span.Center.Longitude);

            MapLayer.SetPosition(pushpin, currentLocation);
            if (Control.Children.Count > 0)
            {
                Control.Children.RemoveAt(0);
            }

            Control.Children.Add(pushpin);
            Control.SetView(currentLocation, 8); // TODO: set zoom level right
        }
    }
}
