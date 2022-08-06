using System.Net;

namespace TestUtilities
{
    public class CodeArmyException : Exception
    {
        public CodeArmyException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; }
    }
}

