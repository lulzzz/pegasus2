using Pegasus.Phone.XF.ViewModels.Views;
using Xamarin.Forms;

namespace Pegasus.Phone.XF
{
	public partial class LocationsView : ContentView
	{
        ILocationsViewSupport map;

		public LocationsView()
		{
			InitializeComponent();
            BindingContext = new LocationsViewModel();
            map = DependencyService.Get<ILocationsViewSupport>();
            map.BindToView(this);
		}
    }
}

