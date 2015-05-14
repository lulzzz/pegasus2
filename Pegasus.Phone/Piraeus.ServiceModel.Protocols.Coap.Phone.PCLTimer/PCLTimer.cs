namespace Piraeus.ServiceModel.Protocols.Coap.Phone.PCLTimer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    public class PCLTimer
    {
        private Timer _timer;

        private Action _action;

        public PCLTimer(Action action, TimeSpan dueTime, TimeSpan period)
        {
            _action = action;

            _timer = new Timer(PCLTimerCallback, null, dueTime, period);
        }

        private void PCLTimerCallback(object state)
        {
            _action.Invoke();
        }

        public bool Change(TimeSpan dueTime, TimeSpan period)
        {
            return _timer.Change(dueTime, period);
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
