using Pegasus2.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegasus.Phone.XF.ViewModels.Views
{
    public class TelemetryDetailsViewModel : BaseViewModel
    {
        static KeyValuePair<string, Func<TelemetryDetailsViewModel, string>>[] formatters =
            new[]
            {
                new KeyValuePair<string, Func<TelemetryDetailsViewModel, string>>(
                    "GPS Altitude", vm => String.Format("{0} ft", vm.CraftTelemetry.Data.GpsAltitude)),
                new KeyValuePair<string, Func<TelemetryDetailsViewModel, string>>(
                    "GPS Longitude", vm => String.Format("{0}", vm.CraftTelemetry.Data.GpsLongitude)),
                new KeyValuePair<string, Func<TelemetryDetailsViewModel, string>>(
                    "GPS Latitude", vm => String.Format("{0}", vm.CraftTelemetry.Data.GpsLatitude))
            };

        public class TelemetryItem : BaseViewModel
        {
            string name;
            public string Name
            {
                get { return name; }
                set { SetProperty(ref name, value); }
            }

            string val;
            public string Value
            {
                get { return val; }
                set { SetProperty(ref val, value); }
            }
        }

        public CraftTelemetryViewModel CraftTelemetry
        {
            get;
            private set;
        }

        public GroundTelemetryViewModel GroundTelemetry
        {
            get;
            private set;
        }

        ObservableCollection<TelemetryItem> telemetryItems;
        public ObservableCollection<TelemetryItem> TelemetryItems
        {
            get { return telemetryItems; }
        }

        public TelemetryDetailsViewModel()
        {
            CraftTelemetry = App.Instance.CurrentCraftTelemetry;
            GroundTelemetry = App.Instance.CurrentGroundTelemetry;

            telemetryItems = new ObservableCollection<TelemetryItem>();
            foreach (var formatter in formatters)
            {
                telemetryItems.Add(new TelemetryItem { Name = formatter.Key });
            }

            // This will cause a memory leak, but since we're only creating one of these...
            CraftTelemetry.PropertyChanged += TelemetryChanged;
            GroundTelemetry.PropertyChanged += TelemetryChanged;
        }

        private void TelemetryChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            for (int i = 0; i < formatters.Length; i++)
            {
                TelemetryItems[i].Name = formatters[i].Key;
                TelemetryItems[i].Value = formatters[i].Value(this);
                //if (i == 2)
                    TelemetryItems[i] = TelemetryItems[i];
            }
        }
    }
}
