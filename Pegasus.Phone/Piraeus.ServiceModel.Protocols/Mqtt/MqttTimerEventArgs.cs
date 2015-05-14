using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piraeus.ServiceModel.Protocols.Mqtt
{
    public class MqttTimerEventArgs : EventArgs
    {
        public MqttTimerEventArgs()
        {
        }

        public MqttTimerEventArgs(ushort messageId, int retryCount)
        {
            this.MessageId = messageId;
            this.RetryCount = retryCount;
        }

        public ushort MessageId { get; internal set; }

        public int RetryCount { get; internal set; }
    }
}
