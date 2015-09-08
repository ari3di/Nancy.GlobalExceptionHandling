using System;

namespace Nancy.GlobalExceptionHandling
{
    [Serializable]
    public class NotHandledException : Exception
    {
        public NotHandledException(string message) : base(message)
        {
        }
    }
}