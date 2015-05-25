using Pegasus2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegasus.Phone.XF.ViewModels.Views
{
    public class LocationsViewModel : BaseViewModel
    {
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

        public LocationsViewModel()
        {
            CraftTelemetry = App.Instance.CurrentCraftTelemetry;
            GroundTelemetry = App.Instance.CurrentGroundTelemetry;
        }
    }
}
