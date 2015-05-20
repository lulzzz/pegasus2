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
    }
}
