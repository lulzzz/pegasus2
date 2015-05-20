using Pegasus.Phone.XF.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Pegasus.Phone.XF
{
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel ViewModel;

        private void ConnectWebSocket(object sender, EventArgs e)
        {
            App.Instance.ConnectWebSocket();
        }

        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = this.ViewModel = new MainPageViewModel();
        }
    }
}
