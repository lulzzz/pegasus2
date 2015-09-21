using Pegasus.Phone.XF.ViewModels.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pegasus.Phone.XF
{
    public partial class TextCraftView : ContentView
    {
        public TextCraftView()
        {
            InitializeComponent();
        }

        private async void SendMessage(object sender = null, EventArgs e = null)
        {
            TextCraftViewModel model = (TextCraftViewModel)BindingContext;
            if (model.CanSubmit)
            {
                App.Instance.AppData.StatusMessage = "Sending...";
                App.Instance.AppData.BusyCount++;

                try
                {
                    string message = model.Message;

                    // This doesn't actually show on Windows.  Buggy Xamarin (last tested on 1.5.0.6446).
                    if (Device.OS == TargetPlatform.Android || Device.OS == TargetPlatform.iOS)
                    {
                        model.Message = String.Empty; 
                    }

                    await App.Instance.SendUserMessageAsync(message);

                    await App.Instance.MainPage.DisplayAlert(String.Empty, "Message sent", "Dismiss");
                }
                finally
                {
                    App.Instance.AppData.StatusMessage = "Sent";
                    App.Instance.AppData.BusyCount--;
                }
            }
 
        }
    }
}
