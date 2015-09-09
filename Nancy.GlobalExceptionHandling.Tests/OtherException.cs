using System;

namespace Nancy.GlobalExceptionHandling.Tests
{
    [Serializable]
    public class OtherException : Exception
    {
        public OtherException(string message) : base(message)
        {
        }
    }
}