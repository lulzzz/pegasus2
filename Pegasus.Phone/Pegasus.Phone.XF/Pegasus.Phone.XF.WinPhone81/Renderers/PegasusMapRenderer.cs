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
using System.Collections.ObjectModel;
using System.Collections.Specialized;

[assembly: ExportRenderer(typeof(Map), typeof(PegasusMapRenderer))]
namespace Pegasus.Phone.XF.WinPhone81.Renderers
{
    public class PegasusMapRenderer : ViewRenderer<Map, MapControl>
    {
        ObservableCollection<Pin> pins;

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
            {
                return;
            }

            var map = new MapControl();
            SetNativeControl(map);

            pins = (ObservableCollection<Pin>)Element.Pins;
            pins.CollectionChanged += Pins_CollectionChanged;

            Xamarin.Forms.MessagingCenter.Subscribe<Map, MapSpan>(
                this, "MapMoveToRegion", OnMoveToRegionMessage, Element);
            OnMoveToRegionMessage(Element, Element.VisibleRegion);
        }

        private void Pins_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // This is definitely not the most efficient -- it may cause us some problems?
            while (Control.MapElements.Count > 0)
            {
                Control.MapElements.RemoveAt(Control.MapElements.Count - 1);
            }

            foreach (Pin pin in pins)
            {
                MapIcon mapIcon = new MapIcon()
                {
                    Title = pin.Label,
                    NormalizedAnchorPoint = new Point(0.5, 0.5),
                    Location = new Geopoint(new BasicGeoposition()
                    {
                        Latitude = pin.Position.Latitude,
                        Longitude = pin.Position.Longitude
                    })
                };
                
                Control.MapElements.Add(mapIcon);
            }
        }

        private void OnMoveToRegionMessage(Map map, MapSpan span)
        {
            if (map == null || span == null)
            {
                return;
            }

            var location = new Geopoint(
              new BasicGeoposition()
              {
                  Latitude = span.Center.Latitude,
                  Longitude = span.Center.Longitude
              });

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
            Control.TrySetViewBoundsAsync(bounds, null, MapAnimationKind.Bow);
       }
   }
}
