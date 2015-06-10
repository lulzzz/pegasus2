using Pegasus.Phone.XF.ViewModels.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pegasus.Phone.XF.Pages
{
    public partial class DetailsPage : ContentPage
    {
        private LocationsViewModel ViewModel;

        public DetailsPage()
        {
            InitializeComponent();
            this.BindingContext = this.ViewModel = new LocationsViewModel();
        }
    }
}
