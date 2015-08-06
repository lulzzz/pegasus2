using Pegasus2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pegasus.Phone.XF.Utilities;
using Xamarin.Forms;

namespace Pegasus.Phone.XF.ViewModels.Views
{
    public class TextCraftViewModel : BaseViewModel
    {
        string message = String.Empty;

        public string Message
        {
            get { return message; }
            set
            {
                SetProperty(ref message, value);
                CanSubmit = false; // value doesn't matter
                ErrorColor = Color.Yellow; // value doesn't matter
                CharactersLeft = -1; // value doesn't matter
            }
        }

        public bool CanSubmit
        {
            get { return !String.IsNullOrEmpty(Message) && Message.Length <= 40; }
            set { OnPropertyChanged(); }
        }

        public Color ErrorColor
        {
            get { return CanSubmit ? Color.White : Color.Red; }
            set { OnPropertyChanged(); }
        }

        public int CharactersLeft
        {
            get { return 40 - (Message ?? String.Empty).Length; }
            set { OnPropertyChanged(); }
        }

        public Command SendCommand
        {
            get { return new Command(SendMessage); }
        }

        private async void SendMessage()
        {
            if (this.CanSubmit)
            {
                await App.Instance.SendUserMessageAsync(this.Message);
            }
        }

        public TextCraftViewModel()
        {
        }
    }
}
