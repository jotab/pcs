using System;
using ApplicationCore.Exceptions;

namespace Infrastructure.Exceptions
{
    [Serializable]
    public class OperatorMappingException : GenericException
    {
        public OperatorMappingException()
            : base("This operator is not supported.")
        {
        }
    }
}