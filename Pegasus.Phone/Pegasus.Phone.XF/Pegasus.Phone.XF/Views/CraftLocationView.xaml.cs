using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Pegasus.Phone.XF
{
	public partial class CraftLocationView : ContentView
	{
        ICraftTelemetryViewSupport map;

		public CraftLocationView()
		{
			InitializeComponent();
            map = DependencyService.Get<ICraftTelemetryViewSupport>();
            map.BindToView(this);
		}
    }
}

