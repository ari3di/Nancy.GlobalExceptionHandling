using System;

namespace Nancy.GlobalExceptionHandling
{
    /// <summary>
    /// This class type would be used in case the throwed excepttion
    /// was not handled by any handlers
    /// </summary>
    [Serializable]
    public class NotHandledException : Exception
    {
        public NotHandledException(string message) : base(message)
        {
        }
    }
}