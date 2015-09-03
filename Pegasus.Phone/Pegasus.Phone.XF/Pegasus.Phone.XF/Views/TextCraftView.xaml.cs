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
                    model.Message = String.Empty; // This doesn't actually show on Windows.  Buggy Xamarin.
                    await App.Instance.SendUserMessageAsync(message);
                    // Make the user wait a bit more...
                    await Task.Delay(500);
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
