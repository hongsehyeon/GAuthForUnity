using System;

namespace GAuthForUnity.Core
{
    public class GAuthException : Exception
    {
        public int StatusCode { get; private set; }

        public GAuthException(int statusCode)
        {
            StatusCode = statusCode;
        }
    }
}