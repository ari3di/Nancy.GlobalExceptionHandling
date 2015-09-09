using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nancy.GlobalExceptionHandling
{
    public static partial class GlobalExceptionHandler
    {
        public static ExceptionHandlerBox ExceptionHandlers { get; private set; }
        static GlobalExceptionHandler()
        {
            ExceptionHandlers = new ExceptionHandlerBox();
        }
        public static Response Handle(NancyContext context, Exception exception)
        {
            var func = ExceptionHandlers.GetHandler(typeof (Exception));

            return func(context.Request);
        }

    }
}
