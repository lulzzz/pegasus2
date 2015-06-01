﻿using Xamarin.Forms.Platform.WinRT;
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

            var map = new MapRenderControl();
            map.Map.Credentials = "Ar63TjGidMOY96jRx8kLubJjOyqKWOI_S3cToA3P0XO9_mQdQEyIowxChrtD9Eii";
            map.MapItems.ItemsSource = Element.Pins;
            SetNativeControl(map);

            Xamarin.Forms.MessagingCenter.Subscribe<XFMap, MapSpan>(
                this, "MapMoveToRegion", OnMoveToRegionMessage, Element);
            OnMoveToRegionMessage(Element, Element.LastMoveToRegion);
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
                span.LatitudeDegrees * 0.75, // The 0.75 defeats the padding built into the map
                span.LongitudeDegrees * 0.75
                );

            Control.Map.SetView(currentLocation);
        }
    }
}
