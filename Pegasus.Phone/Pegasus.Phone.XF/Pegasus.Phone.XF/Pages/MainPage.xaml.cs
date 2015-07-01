using Pegasus.Phone.XF.Utilities;
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
        private MainPageViewModel viewModel;
        private Dictionary<Button, View> buttons = new Dictionary<Button, View>();

        private void SwitchToView(object sender, EventArgs e = null)
        {
            foreach (var kvp in buttons)
            {
                bool match = (kvp.Key == sender);
                kvp.Key.IsEnabled = !match;
                kvp.Value.IsVisible = match;

                if (match)
                {
                    Settings.HomePageView = kvp.Value.GetType().Name;
                }
            }
        }

        private async void DoTest(object sender = null, EventArgs e = null)
        {
            await App.Instance.FakeLocationAsync();
        }

        public MainPage()
        {
            InitializeComponent();

            this.BindingContext = this.viewModel = new MainPageViewModel();
            this.viewModel.AppData.PropertyChanged += AppData_PropertyChanged;
            this.buttons[this.TelemetryOverviewButton] = this.TelemetryOverviewView;
            this.buttons[this.LocationsButton] = this.LocationsView;
            this.buttons[this.TelemetryDetailsButton] = this.TelemetryDetailsView;
            this.buttons[this.TextCraftButton] = this.TextCraftView;

            string defaultView = Settings.HomePageView;
            Button defaultButton = this.buttons.FirstOrDefault(kvp => kvp.Value.GetType().Name == defaultView).Key ?? this.buttons.First().Key;
            this.SwitchToView(defaultButton);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await App.Instance.ConnectWebSocketAsync();
        }

        private async void AppData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsBusy")
            {
                if (this.viewModel.AppData.IsBusy)
                {
                    this.ActivityIndicator.IsRunning = true;
                    this.ActivityIndicatorBackground.Opacity = 0;
                    this.ActivityIndicatorBackground.IsVisible = true;
                    await this.ActivityIndicatorBackground.FadeTo(0.75);
                }
                else
                {
                    this.ActivityIndicator.IsRunning = false;
                    await this.ActivityIndicatorBackground.FadeTo(0);
                    this.ActivityIndicatorBackground.IsVisible = false;
                }
            }
        }
    }
}
