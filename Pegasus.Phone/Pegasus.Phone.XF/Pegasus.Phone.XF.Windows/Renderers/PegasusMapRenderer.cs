using Xamarin.Forms.Platform.WinRT;
using Bing.Maps;
using BingMap = Bing.Maps.Map;
using Pegasus.Phone.XF.Windows.Renderers;
using Xamarin.Forms.Maps;
using XFMap = Xamarin.Forms.Maps.Map;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

// https://visualstudiogallery.msdn.microsoft.com/224eb93a-ebc4-46ba-9be7-90ee777ad9e1

[assembly: ExportRenderer(typeof(XFMap), typeof(PegasusMapRenderer))]
namespace Pegasus.Phone.XF.Windows.Renderers
{
    public class PegasusMapRenderer : ViewRenderer<XFMap, BingMap>
    {
        ObservableCollection<Pin> pins;

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

            pins = (ObservableCollection<Pin>)Element.Pins;
            pins.CollectionChanged += Pins_CollectionChanged;

            Xamarin.Forms.MessagingCenter.Subscribe<XFMap, MapSpan>(
                this, "MapMoveToRegion", OnMoveToRegionMessage, Element);
            OnMoveToRegionMessage(Element, Element.VisibleRegion);
        }

        private void Pins_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // This is definitely not the most efficient -- it may cause us some problems?
            while (Control.Children.Count > 0)
            {
                Control.Children.RemoveAt(Control.Children.Count - 1);
            }

            foreach (Pin pin in pins)
            {
                Pushpin pushPin = new Pushpin();
                pushPin.Text = pin.Label;

                Location pinLocation = new Location(
                    pin.Position.Latitude,
                    pin.Position.Longitude);

                MapLayer.SetPosition(pushPin, pinLocation);

                 Control.Children.Add(pushPin);
            }
        }

        private void OnMoveToRegionMessage(XFMap map, MapSpan span)
        {
            if (map == null || span == null)
            {
                return;
            }

            Location currentLocation = new Location(
                span.Center.Latitude,
                span.Center.Longitude);

           Control.SetView(currentLocation, 8); // TODO: set zoom level right
        }
    }
}
