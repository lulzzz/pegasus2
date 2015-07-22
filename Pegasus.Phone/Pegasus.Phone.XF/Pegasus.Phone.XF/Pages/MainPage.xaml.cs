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
        private Dictionary<Image, View> images = new Dictionary<Image, View>();

        private void SwitchToView(object sender, EventArgs e = null)
        {
            foreach (var kvp in images)
            {
                bool match = (kvp.Key == sender);
                kvp.Key.Opacity = match ? 1.0 : 0.5;
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
            this.images[this.TelemetryOverviewImage] = this.TelemetryOverviewView;
            this.images[this.MapImage] = this.LocationsView;
            this.images[this.TelemetryDetailsImage] = this.TelemetryDetailsView;
            this.images[this.TextCraftImage] = this.TextCraftView;
            this.images[this.LinksImage] = this.LinksView;

            TapGestureRecognizer tapImage = new TapGestureRecognizer();
            tapImage.Tapped += SwitchToView;
            foreach (Image image in this.images.Keys)
            {
                image.GestureRecognizers.Add(tapImage);
            }

            string defaultView = Settings.HomePageView;
            Image defaultImage = this.images.FirstOrDefault(kvp => kvp.Value.GetType().Name == defaultView).Key ?? this.images.First().Key;
            this.SwitchToView(defaultImage);
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
