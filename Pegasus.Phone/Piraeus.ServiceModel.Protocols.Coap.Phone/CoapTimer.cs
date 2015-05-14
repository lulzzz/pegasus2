
namespace Piraeus.ServiceModel.Protocols.Coap
{
    using Piraeus.ServiceModel.Protocols.Coap.Phone.PCLTimer;
    using System;
    using System.Threading;

    public delegate void CoAPTimerEventHandler(object sender, CoapTimerArgs args);
    public class CoapTimer
    {
        public CoapTimer(CoapMessage message, string internalMessageId)
        {
            this.message = message;
            this.internalMessageId = internalMessageId;
            this.interval = Convert.ToInt32(CoapConstants.Timeouts.AckTimeout.Milliseconds) * Convert.ToInt32(CoapConstants.Timeouts.AckRandomFactor);
            //this.timer = new Timer(interval);
            
            //TimerCallback tc = new TimerCallback(timer_Elapsed);

            this.timer = new PCLTimer(new Action(timer_Elapsed), TimeSpan.FromMilliseconds(5000), TimeSpan.FromSeconds(interval));
           

            //this.timer.Elapsed += timer_Elapsed;
            this.startTime = DateTime.Now;
            //this.timer.Start();
            
        }

        public event CoAPTimerEventHandler Timeout;
        
        private int interval;
        private PCLTimer timer;
        private int retryAttempt;
        private CoapMessage message;
        private DateTime startTime;
        private string internalMessageId;
        public void Decrement()
        {
            retryAttempt++;
            if (retryAttempt < CoapConstants.Timeouts.MaxRetransmit)
            {
                this.interval = this.interval * 2;
                this.timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(this.interval));
                //this.timer.Interval = this.interval;
                
            }
        }
        public void Stop()
        {
            //this.timer.Stop();
            this.timer.Dispose();
            
        }


        void timer_Elapsed()
        {
            if (Timeout != null)
            {
                Timeout(this, new CoapTimerArgs(this.retryAttempt, this.startTime, this.message, this.internalMessageId));
            }
        }
    }
}
