using System;

namespace Nancy.GlobalExceptionHandling.Tests
{
    [Serializable]
    public class SomeException : Exception
    {
        public SomeException(string message) : base(message)
        {
        }
    }
}