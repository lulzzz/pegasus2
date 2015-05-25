using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms.Platform.WinRT;
using Windows.Devices.Geolocation;
using Pegasus.Phone.XF.WinPhone81.Renderers;
using Windows.Foundation;
using Xamarin.Forms.Maps;

[assembly: ExportRenderer(typeof(Map), typeof(PegasusMapRenderer))]
namespace Pegasus.Phone.XF.WinPhone81.Renderers
{
    public class PegasusMapRenderer : ViewRenderer<Map, MapControl>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
            {
                return;
            }

            var map = new MapControl();
            SetNativeControl(map);

            Xamarin.Forms.MessagingCenter.Subscribe<Map, MapSpan>(
                this, "MapMoveToRegion", OnMoveToRegionMessage, Element);
        }

        private void OnMoveToRegionMessage(Map map, MapSpan span)
        {
            if (map == null || span == null)
            {
                return;
            }

            MapIcon mapIcon = new MapIcon();
            mapIcon.Title = "Current Location";
            mapIcon.Location = new Geopoint(
              new BasicGeoposition()
              {
                  Latitude = span.Center.Latitude,
                  Longitude = span.Center.Longitude
              });

            mapIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
            if (Control.MapElements.Count > 0)
            {
                Control.MapElements.RemoveAt(0);
            }

            Control.MapElements.Add(mapIcon);
            Control.TrySetViewAsync(mapIcon.Location, 12D, 0, 0, MapAnimationKind.Bow);
       }
   }
}
