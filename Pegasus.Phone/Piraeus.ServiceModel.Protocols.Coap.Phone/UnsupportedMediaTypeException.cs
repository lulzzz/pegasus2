
namespace Piraeus.ServiceModel.Protocols.Coap
{
    using System;
    using System.Runtime.Serialization;
    public class UnsupportedMediaTypeException : Exception
    {
        public UnsupportedMediaTypeException()
            : base()
        {
        }

        public UnsupportedMediaTypeException(string message)
            : base(message)
        {
        }

        public UnsupportedMediaTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}
