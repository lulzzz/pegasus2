using Pegasus.Phone.XF.ViewModels.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Pegasus.Phone.XF
{
    public partial class TextCraftView : ContentView
    {
        public TextCraftView()
        {
            InitializeComponent();
        }

        private void SendUserMessage(object sender, EventArgs e)
        {
            //this.SendMessageButton.IsEnabled = false;
            if (!String.IsNullOrEmpty(this.MessageEditor.Text))
            {
                //App.Instance.ConnectWebSocket();
                App.Instance.TriggerSendUserMessage(this.MessageEditor.Text);
            }
        }
    }

    public class EditorValidation : TriggerAction<Editor> {
        protected override void Invoke ( Editor sender ) {
            bool valid = sender.Text.Length < 40;
            if (!valid)
            {
                sender.BackgroundColor = Color.Red;
            }
            else
            {
                sender.BackgroundColor = Color.White;
            }
        }
    }
}
