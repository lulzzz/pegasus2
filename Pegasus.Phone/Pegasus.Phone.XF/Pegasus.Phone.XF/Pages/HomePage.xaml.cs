using Pegasus.Phone.XF.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pegasus.Phone.XF.Pages
{
    public partial class HomePage : ContentPage
    {
        private MainPageViewModel ViewModel;

        public HomePage()
        {
            InitializeComponent();
            this.BindingContext = this.ViewModel = new MainPageViewModel();
        }

        private void ConnectWebSocket(object sender, EventArgs e)
        {
            this.ConnectButton.IsEnabled = false;
            App.Instance.ConnectWebSocket();
        }
    }
}
