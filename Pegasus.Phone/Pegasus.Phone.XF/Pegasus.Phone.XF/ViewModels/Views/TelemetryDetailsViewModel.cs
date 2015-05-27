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

            internal Func<TelemetryDetailsViewModel, string> Formatter;
        }

        public CraftTelemetryViewModel CraftTelemetry
        {
            get;
            private set;
        }

        ObservableCollection<TelemetryItem> telemetryItems;
        public ObservableCollection<TelemetryItem> TelemetryItems
        {
            get { return telemetryItems; }
        }

        private void TelemetryChanged(object sender = null, System.ComponentModel.PropertyChangedEventArgs e = null)
        {
            foreach (var item in TelemetryItems)
            {
                item.Value = (this.CraftTelemetry.Data == null) ? "-" : item.Formatter(this);
            }
        }

        public TelemetryDetailsViewModel()
        {
            CraftTelemetry = App.Instance.CurrentCraftTelemetry;

            telemetryItems = new ObservableCollection<TelemetryItem>();
            telemetryItems.Add(new TelemetryItem
            {
                Name = "GPS Alitutude",
                Formatter = vm => String.Format("{0} ft", vm.CraftTelemetry.Data.GpsAltitude)
            });
            telemetryItems.Add(new TelemetryItem
            {
                Name = "GPS Longitude",
                Formatter = vm => String.Format("{0}", vm.CraftTelemetry.Data.GpsLongitude)
            });

            // This will cause a memory leak, but since we're only creating one of these...
            CraftTelemetry.PropertyChanged += TelemetryChanged;
            TelemetryChanged();
        }
    }
}
