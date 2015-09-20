using Pegasus2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegasus.Phone.XF.ViewModels
{
    public class AppDataViewModel : BaseViewModel
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

        private int busyCount;
        public int BusyCount
        {
            get { return busyCount; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                SetProperty(ref busyCount, value);
                IsBusy = true; // value doesn't matter
            }
        }

        public bool IsBusy
        {
            get { return busyCount != 0; }
            private set { OnPropertyChanged(); }
        }

        internal LaunchInfo launchInfo;
        public LaunchInfo LaunchInfo
        {
            get { return launchInfo; }
            set { SetProperty(ref launchInfo, value); }
        }
    }
}
