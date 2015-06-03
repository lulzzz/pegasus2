using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms.Platform.WinRT;
using Windows.Devices.Geolocation;
using Pegasus.Phone.XF.WinPhone81.Renderers;
using Windows.Foundation;
using Xamarin.Forms.Maps;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Pegasus.Phone.XF.WinPhone81.Controls;

[assembly: ExportRenderer(typeof(Map), typeof(PegasusMapRenderer))]
namespace Pegasus.Phone.XF.WinPhone81.Renderers
{
    public class PegasusMapRenderer : ViewRenderer<Map, MapRenderControl>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
            {
                return;
            }

            SetNativeControl(new MapRenderControl());
            Control.MapItems.ItemsSource = Element.Pins;
            Control.Map.MapServiceToken = "AtxO1mWPBN-w3GVYC_kQoOf50VRIupvCWy8NV0-WcmhiRai2OExiOcKXQpLEtwJr";

            Xamarin.Forms.MessagingCenter.Subscribe<Map, MapSpan>(
                this, "MapMoveToRegion", OnMoveToRegionMessage, Element);
            OnMoveToRegionMessage(Element, Element.LastMoveToRegion);
        }

        private void OnMoveToRegionMessage(Map map, MapSpan span)
        {
            if (map == null || span == null || (span.LatitudeDegrees == 0.1 && span.LongitudeDegrees == 0.1))
            {
                return;
            }

            // These calculations only work in the northwest quadrant of the world
            var northWestCorner = new BasicGeoposition()
            {
                Latitude = span.Center.Latitude + span.LatitudeDegrees / 2,
                Longitude = span.Center.Longitude - span.LongitudeDegrees / 2
            };
            var southEastCorner = new BasicGeoposition()
            {
                Latitude = span.Center.Latitude - span.LatitudeDegrees / 2,
                Longitude = span.Center.Longitude + span.LongitudeDegrees / 2
            };
            var bounds = new GeoboundingBox(northWestCorner, southEastCorner);
            Control.Map.TrySetViewBoundsAsync(bounds, null, MapAnimationKind.Bow);
       }
   }
}
