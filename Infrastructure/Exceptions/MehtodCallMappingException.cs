using System;
using ApplicationCore.Exceptions;

namespace Infrastructure.Exceptions
{
    [Serializable]
    public class MehtodCallMappingException : GenericException
    {
        public MehtodCallMappingException()
            : base("This method call is not supported.")
        {
        }
    }
}