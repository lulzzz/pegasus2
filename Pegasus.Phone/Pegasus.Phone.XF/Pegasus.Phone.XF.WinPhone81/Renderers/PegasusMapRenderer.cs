using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms.Platform.WinRT;
using Windows.Devices.Geolocation;
using Pegasus.Phone.XF.WinPhone81.Renderers;
using Windows.Foundation;
using Xamarin.Forms.Maps;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Pegasus.Phone.XF.WinPhone81.Controls;
using System;
using System.Diagnostics;

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
            Control.Map.MapServiceToken = "AugsM2wpsu3p8J7azxN1cM0L79uRA6N9-Dy8DW6uMStv0tcz1CA-vVsAVRozJxDG";
            Control.MapItems.ItemsSource = Element.Pins;
            Control.Map.CenterChanged += (s, e2) => SetVisibleRegion();
            Control.Map.ZoomLevelChanged += (s, e2) => SetVisibleRegion();
            Control.Map.LoadingStatusChanged += (s, e2) => SetVisibleRegion();

            Xamarin.Forms.MessagingCenter.Subscribe<Map, MapSpan>(
                this, "MapMoveToRegion", OnMoveToRegionMessage, Element);
            OnMoveToRegionMessage(Element, Element.LastMoveToRegion);
        }

        private void SetVisibleRegion()
        {
            try
            {
                Geopoint northWestCorner;
                try
                {
                    Control.Map.GetLocationFromOffset(new Point(0, 0), out northWestCorner);
                }
                catch (ArgumentException)
                {
                    var topOfMap = new Geopoint(new BasicGeoposition()
                    {
                        Latitude = 85,
                        Longitude = 0
                    });

                    Windows.Foundation.Point topPoint;
                    Control.Map.GetOffsetFromLocation(topOfMap, out topPoint);
                    Control.Map.GetLocationFromOffset(new Windows.Foundation.Point(0, topPoint.Y), out northWestCorner);
                }

                Geopoint southEastCorner = null;
                try
                {
                    Control.Map.GetLocationFromOffset(new Windows.Foundation.Point(Control.Map.ActualWidth, Control.Map.ActualHeight), out southEastCorner);
                }
                catch (ArgumentException)
                {
                    var bottomOfMap = new Geopoint(new BasicGeoposition()
                    {
                        Latitude = -85,
                        Longitude = 0
                    });

                    Windows.Foundation.Point bottomPoint;
                    Control.Map.GetOffsetFromLocation(bottomOfMap, out bottomPoint);
                    Control.Map.GetLocationFromOffset(new Windows.Foundation.Point(0, bottomPoint.Y), out southEastCorner);
                }

                Element.VisibleRegion = new MapSpan(
                    new Position(Control.Map.Center.Position.Latitude, Control.Map.Center.Position.Longitude),
                    Math.Abs(Control.Map.Center.Position.Latitude - northWestCorner.Position.Latitude) * 2,
                    Math.Abs(Control.Map.Center.Position.Longitude - northWestCorner.Position.Longitude) * 2);
            }
            catch (ArgumentException)
            {
            }
        }

        private void OnMoveToRegionMessage(Map map, MapSpan span)
        {
            if (map == null || span == null || (span.LatitudeDegrees == 0.1 && span.LongitudeDegrees == 0.1))
            {
                return;
            }

            // These calculations only work in the northwest quadrant of the world
            double scaleFactor = 0.8097419383212764;
            var northWestCorner = new BasicGeoposition()
            {
                Latitude = span.Center.Latitude + (span.LatitudeDegrees / 2)*scaleFactor,
                Longitude = span.Center.Longitude - (span.LongitudeDegrees / 2)*scaleFactor
            };
            var southEastCorner = new BasicGeoposition()
            {
                Latitude = span.Center.Latitude - (span.LatitudeDegrees / 2)*scaleFactor,
                Longitude = span.Center.Longitude + (span.LongitudeDegrees / 2)*scaleFactor
            };
            var bounds = new GeoboundingBox(northWestCorner, southEastCorner);
            // Because this control only can tell us what *is* visible, not the target of the animation,
            // we can't reliably know what the view is if we have any animations going.  Yuck.
            Control.Map.TrySetViewBoundsAsync(bounds, null, MapAnimationKind.None);
       }
   }
}
