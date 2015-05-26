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
        private Dictionary<Button, View> buttons = new Dictionary<Button, View>();

        private void ConnectWebSocket(object sender, EventArgs e)
        {
            App.Instance.ConnectWebSocket();
        }

        private void SwitchToView(object sender, EventArgs e = null)
        {
            foreach (var kvp in buttons)
            {
                bool match = (kvp.Key == sender);
                kvp.Key.IsEnabled = !match;
                kvp.Value.IsVisible = match;
            }
        }

        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = this.ViewModel = new MainPageViewModel();
            this.buttons[this.TelemetryOverviewButton] = this.TelemetryOverviewView;
            this.buttons[this.LocationsButton] = this.LocationsView;
            this.buttons[this.TelemetryDetailsButton] = this.TelemetryDetailsView;
            this.SwitchToView(this.buttons.First().Key);
        }
    }
}
