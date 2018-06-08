using System;
using System.Runtime.Serialization;

namespace ApplicationCore.Exceptions
{
    [Serializable]
    public class GenericException : Exception
    {
        public GenericException()
        {
        }

        public GenericException(string message)
            : base(message)
        {
        }

        public GenericException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public GenericException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public GenericException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException)
        {
        }

        protected GenericException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}