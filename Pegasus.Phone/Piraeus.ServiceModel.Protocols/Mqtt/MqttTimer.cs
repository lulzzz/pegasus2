
using System;
using System.Threading;

namespace Piraeus.ServiceModel.Protocols.Mqtt
{
    public class MqttTimer
    {
        public event EventHandler OnExpired;


        public MqttTimer(int periodMilliseconds)
        {
            this.period = periodMilliseconds;
        }

        public MqttTimer(int periodMilliseconds, ushort messageId)
        {
            this.period = periodMilliseconds;
            this.messageId = messageId;
        }

        private Timer timer;
        private int period;
        private ushort messageId;
        private int retryCount;

        public void Start()
        {
            this.timer = new Timer(new TimerCallback(Callback), null, 0, this.period);
        }

        public void Stop()
        {
            this.timer.Dispose();
        }

        public void Reset()
        {
            this.timer.Dispose();
            this.timer = new Timer(new TimerCallback(Callback), null, 0, period);
        }

        public void Callback(object state)
        {
            if(this.OnExpired != null)
            {
                this.retryCount++;
                OnExpired(this, new MqttTimerEventArgs(this.messageId, this.retryCount));
            }
        }
    }
}
