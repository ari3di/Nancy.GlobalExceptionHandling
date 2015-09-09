using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nancy.GlobalExceptionHandling
{
    public partial class GlobalExceptionHandler
    {
        public ExceptionHandlerBox Handlers { get; private set; }
        public GlobalExceptionHandler()
        {
            Handlers = new ExceptionHandlerBox();
        }
        public Response Handle(NancyContext context, Exception exception)
        {
            var func = Handlers.Get(typeof (Exception));

            return func(context, exception);
        }

    }
}
