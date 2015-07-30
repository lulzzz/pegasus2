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
        public CraftTelemetryViewModel CraftTelemetry
        {
            get;
            private set;
        }

        public TelemetryDetailsViewModel()
        {
            CraftTelemetry = App.Instance.CurrentCraftTelemetry;
       }
    }
}
