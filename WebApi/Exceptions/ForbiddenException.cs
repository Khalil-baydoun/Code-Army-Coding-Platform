using System;
using DataContracts.Authentication;

namespace WebApi.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenReason? ForbiddenReason { get; set; }

        public ForbiddenException(string message, ForbiddenReason? forbiddenReason) : base(message)
        {
            ForbiddenReason = forbiddenReason;
        }
        public ForbiddenException(string message, Exception innerException, ForbiddenReason? forbiddenReason) : base(message, innerException)
        {
            ForbiddenReason = forbiddenReason;
        }
    }
}
