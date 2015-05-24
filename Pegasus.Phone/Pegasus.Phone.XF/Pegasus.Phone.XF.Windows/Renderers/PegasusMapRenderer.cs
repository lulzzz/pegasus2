using Pegasus.Phone.XF.CustomControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.WinRT;
using Bing.Maps;
using System.ComponentModel;
using Pegasus.Phone.XF.Windows.Renderers;

[assembly: ExportRenderer(typeof(PegasusMap), typeof(PegasusMapRenderer))]
namespace Pegasus.Phone.XF.Windows.Renderers
{
    public class PegasusMapRenderer : ViewRenderer<PegasusMap, Map>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<PegasusMap> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
                return;
            var map = new Map();
            map.Credentials = "Ar63TjGidMOY96jRx8kLubJjOyqKWOI_S3cToA3P0XO9_mQdQEyIowxChrtD9Eii";
            SetNativeControl(map);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (this.Element == null || this.Control == null)
                return;
            if (e.PropertyName == PegasusMap.CenterPositionProperty.PropertyName)
            {
                Pushpin pushpin = new Pushpin();
                pushpin.Text = "Current Location";
                Location currentLocation = new Location(Element.CenterPosition.Latitude,
                    Element.CenterPosition.Longitude);
                MapLayer.SetPosition(pushpin, currentLocation);
                if (Control.Children.Count > 0)
                    Control.Children.RemoveAt(0);
                Control.Children.Add(pushpin);
                Control.SetView(currentLocation, 8);
            }
            else
            { }
        }
    }
}
