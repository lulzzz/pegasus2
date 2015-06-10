using Pegasus2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pegasus.Phone.XF.Utilities;

namespace Pegasus.Phone.XF.ViewModels.Views
{
    public class LocationsViewModel : BaseViewModel
    {
        public CraftTelemetryViewModel CraftTelemetry
        {
            get;
            private set;
        }

        public GroundTelemetryViewModel ChaseTelemetry
        {
            get;
            private set;
        }

        public GroundTelemetryViewModel LaunchTelemetry
        {
            get;
            private set;
        }

        public String ChaseVehicleToCraftDistanceText
        {
            get
            {
                if (CraftTelemetry.Data == null || ChaseTelemetry.Data == null)
                {
                    return null;
                }

                return String.Format(
                    "Chase Vehicle to Craft: {0:0.00}km",
                    Math.Abs(CraftTelemetry.Data.ToPosition().DistanceFrom(
                             ChaseTelemetry.Data.ToPosition()).Kilometers));
            }

            // used only to generate notifications
            set
            {
                string unused = "bogus value";
                SetProperty(ref unused, value);
            }
        }

        public LocationsViewModel()
        {
            CraftTelemetry = App.Instance.CurrentCraftTelemetry;
            ChaseTelemetry = App.Instance.CurrentChaseTelemetry;
            LaunchTelemetry = App.Instance.CurrentLaunchTelemetry;

            ChaseTelemetry.PropertyChanged += TelemetryChanged;
            CraftTelemetry.PropertyChanged += TelemetryChanged;
        }

        private void TelemetryChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Generate notifications
            ChaseVehicleToCraftDistanceText = null;
        }
    }
}
