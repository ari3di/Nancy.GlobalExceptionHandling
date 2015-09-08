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
        public static ErrorBoks ErrorBox { get; private set; }
        static GlobalExceptionHandler()
        {
            ErrorBox = new ErrorBoks();
        }
        public static Response Handle(NancyContext context, Exception exception)
        {
            var func = ErrorBox.Get(typeof (Exception));

            return func(context.Request);
        }

    }
}
