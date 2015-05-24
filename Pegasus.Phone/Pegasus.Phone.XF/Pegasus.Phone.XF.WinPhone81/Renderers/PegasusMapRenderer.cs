using Pegasus.Phone.XF.CustomControl;
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

[assembly: ExportRenderer(typeof(PegasusMap), typeof(PegasusMapRenderer))]
namespace Pegasus.Phone.XF.WinPhone81.Renderers
{
    public class PegasusMapRenderer : ViewRenderer<PegasusMap, MapControl>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<PegasusMap> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
                return;
            var map = new MapControl();
            SetNativeControl(map);
        }

        protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e) {
          base.OnElementPropertyChanged (sender, e);
          if (this.Element == null || this.Control == null)
            return;
          if (e.PropertyName == PegasusMap.CenterPositionProperty.PropertyName) {
              MapIcon mapIcon = new MapIcon();
              mapIcon.Title = "Current Location";
              mapIcon.Location = new Geopoint(
                new BasicGeoposition()
                {
                    Latitude = Element.CenterPosition.Latitude,
                    Longitude = Element.CenterPosition.Longitude
                });
              mapIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
              if (Control.MapElements.Count > 0)
                  Control.MapElements.RemoveAt(0);
              Control.MapElements.Add(mapIcon);
              Control.TrySetViewAsync(mapIcon.Location, 12D, 0, 0, MapAnimationKind.Bow);
          } 
          else
          {}
        }
    }
}
