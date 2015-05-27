using Pegasus.Phone.XF.ViewModels.Views;
using Pegasus2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegasus.Phone.XF.ViewModels.Pages
{
    class MainPageViewModel : BasePageViewModel
    {
        public CraftTelemetryViewModel CraftTelemetry
        {
            get { return App.Instance.CurrentCraftTelemetry; }
        }

        private LocationsViewModel locationsViewModel;
        public LocationsViewModel LocationsViewModel
        {
            get
            {
                if (locationsViewModel == null)
                {
                    locationsViewModel = new LocationsViewModel();
                }

                return locationsViewModel;
            }
        }

        private TelemetryDetailsViewModel telemetryDetailsViewModel;
        public TelemetryDetailsViewModel TelemetryDetailsViewModel
        {
            get
            {
                if (telemetryDetailsViewModel == null)
                {
                    telemetryDetailsViewModel = new TelemetryDetailsViewModel();
                }

                return telemetryDetailsViewModel;
            }
        }

        public MainPageViewModel()
        {
        }
    }
}
