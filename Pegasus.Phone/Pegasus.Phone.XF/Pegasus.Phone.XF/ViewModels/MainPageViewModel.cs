using Pegasus2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegasus.Phone.XF.ViewModels
{
    class MainPageViewModel : BasePageViewModel
    {
        private string statusMessage;
        public string StatusMessage
        {
            get { return statusMessage; }
            set { SetProperty(ref statusMessage, value); }
        }

        private int messageCount;
        public int MessageCount
        {
            get { return messageCount; }
            set { SetProperty(ref messageCount, value); }
        }

        private CraftTelemetry craftTelemetry;
        public CraftTelemetry CraftTelemetry
        {
            get { return craftTelemetry; }
            set { SetProperty(ref craftTelemetry, value); }
        }

        public MainPageViewModel()
        {
            StatusMessage = "App launched, press Go!";
        }
    }
}
