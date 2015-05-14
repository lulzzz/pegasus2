

namespace Piraeus.ServiceModel.Protocols.Coap
{
    using System;
    using System.Runtime.Serialization;
    public class CoapVersionMismatchException : Exception
    {
        public CoapVersionMismatchException()
        {
        }

        public CoapVersionMismatchException(string message)
            : base(message)
        {
        }

        public CoapVersionMismatchException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}
