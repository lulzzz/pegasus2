using Pegasus.Phone.XF.ViewModels.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pegasus.Phone.XF.Pages
{
    public partial class TextCraftPage : ContentPage
    {
        private LocationsViewModel viewModel;

        public TextCraftPage()
        {
            InitializeComponent();
            this.BindingContext = this.viewModel = new LocationsViewModel();
        }
    }
}
