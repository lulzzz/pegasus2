using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegasus.Phone.XF.ViewModels
{
    public class BasicViewModel<T> : BaseViewModel
    {
        private T data;
        public T Data
        {
            get { return data; }
            set { SetProperty(ref data, value); OnSetData(); }
        }

        protected BasicViewModel()
        {
        }

        protected virtual void OnSetData()
        {
        }
    }
}
