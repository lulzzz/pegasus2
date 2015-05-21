using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Pegasus.Phone.XF
{
	public partial class CraftLocationView : ContentView
	{
        IMapsSupport map;

		public CraftLocationView()
		{
			InitializeComponent();
            map = DependencyService.Get<IMapsSupport>();
            map.BindToView(this);
		}
    }
}

