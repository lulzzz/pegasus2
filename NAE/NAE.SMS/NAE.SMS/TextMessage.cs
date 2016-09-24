using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;

namespace NAE.SMS
{
    public class TextMessage
    {
        private TwilioRestClient twilio;
        private string twilioPhoneNumber;
        public TextMessage()
        {
            string accountSid = ConfigurationManager.AppSettings.Get("TwilioAccountSid");
            string authToken = ConfigurationManager.AppSettings.Get("TwilioAuthToken");
            this.twilioPhoneNumber = ConfigurationManager.AppSettings.Get("TwilioPhoneNumber");
            this.twilio = new TwilioRestClient(accountSid, authToken);
        }
        public async Task NotifyAsync(string message, string phone)
        {
            try
            {
                this.twilio.SendMessage(twilioPhoneNumber, phone, message);
                                
                //await Retry.ExecuteAsync(() =>
                //{
                    
                //}, TimeSpan.FromSeconds(2), 3);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning(String.Format("Twilio message failed. {0}", phone));
                Trace.TraceError(ex.Message);
            }
        }
    }
}
