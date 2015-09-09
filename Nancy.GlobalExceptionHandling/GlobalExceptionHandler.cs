using System;

namespace Nancy.GlobalExceptionHandling
{
    public static class GlobalExceptionHandler
    {
        public static ExceptionHandlerBox Handlers { get; private set; }
        static GlobalExceptionHandler()
        {
            Handlers = new ExceptionHandlerBox();
        }
        public static Response Handle(NancyContext context, Exception exception)
        {
            var handler = Handlers.Get(typeof (Exception));

            return handler(context, exception);
        }

    }
}
