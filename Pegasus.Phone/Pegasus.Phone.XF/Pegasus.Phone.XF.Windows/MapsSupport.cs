using Pegasus.Phone.XF.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

[assembly: Xamarin.Forms.Dependency(typeof(MapsSupport))]
namespace Pegasus.Phone.XF.Windows
{
    public class MapsSupport : IMapsSupport
    {
        Map map;

        public void BindToView(ContentView view)
        {
            map = new Xamarin.Forms.Maps.Map();
            view.Content = map;
        }
    }
}
