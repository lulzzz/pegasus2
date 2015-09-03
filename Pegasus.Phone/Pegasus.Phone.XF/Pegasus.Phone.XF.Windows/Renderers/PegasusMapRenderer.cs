using Xamarin.Forms.Platform.WinRT;
using Bing.Maps;
using Pegasus.Phone.XF.Windows.Renderers;
using Xamarin.Forms.Maps;
using XFMap = Xamarin.Forms.Maps.Map;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Pegasus.Phone.XF.Windows.Controls;

// https://visualstudiogallery.msdn.microsoft.com/224eb93a-ebc4-46ba-9be7-90ee777ad9e1

[assembly: ExportRenderer(typeof(XFMap), typeof(PegasusMapRenderer))]
namespace Pegasus.Phone.XF.Windows.Renderers
{
    public class PegasusMapRenderer : ViewRenderer<XFMap, MapRenderControl>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<XFMap> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
            {
                return;
            }

            SetNativeControl(new MapRenderControl());
            Control.Map.Credentials = "AugsM2wpsu3p8J7azxN1cM0L79uRA6N9-Dy8DW6uMStv0tcz1CA-vVsAVRozJxDG";
            Control.MapItems.ItemsSource = Element.Pins;
            Control.Map.ViewChanged += Map_ViewChanged;

            Xamarin.Forms.MessagingCenter.Subscribe<XFMap, MapSpan>(
                this, "MapMoveToRegion", OnMoveToRegionMessage, Element);
            OnMoveToRegionMessage(Element, Element.LastMoveToRegion);
        }

        private void Map_ViewChanged(object sender, ViewChangedEventArgs e)
        {
            Element.VisibleRegion = new MapSpan(
                new Position(Control.Map.Bounds.Center.Latitude, Control.Map.Bounds.Center.Longitude),
                Control.Map.Bounds.Width, Control.Map.Bounds.Height);
        }

        private void OnMoveToRegionMessage(XFMap map, MapSpan span)
        {
            if (map == null || span == null || (span.LatitudeDegrees == 0.1 && span.LongitudeDegrees == 0.1))
            {
                return;
            }

            LocationRect currentLocation = new LocationRect(
                new Location(
                    span.Center.Latitude,
                    span.Center.Longitude),
                span.LatitudeDegrees,
                span.LongitudeDegrees
                );

            Control.Map.SetView(currentLocation);
        }
    }
}
