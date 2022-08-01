using System;
using DataContracts.Submissions;

namespace WebApi.Exceptions
{
    public class ProcessFailedException : Exception
    {
        public Verdict Verdict { get; set; }
        public ProcessFailedException(Verdict verdict)
        {
            Verdict = verdict;
        }

        public ProcessFailedException(string message) : base(message)
        {
        }

        public ProcessFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

