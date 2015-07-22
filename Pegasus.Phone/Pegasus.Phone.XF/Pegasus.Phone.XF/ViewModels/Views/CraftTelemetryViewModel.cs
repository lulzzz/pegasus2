using Pegasus2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegasus.Phone.XF.ViewModels.Views
{
    public class CraftTelemetryViewModel : BasicViewModel<CraftTelemetry>
    {
        public string TimestampInHhmm
        {
            get { return (Data == null) ? null : Data.Timestamp.ToString("HH:mm"); }
            set { OnPropertyChanged(); }
        }

        public CraftTelemetryViewModel() { }

        protected override void OnSetData()
        {
            base.OnSetData();
            this.TimestampInHhmm = null;
        }
    }
}
