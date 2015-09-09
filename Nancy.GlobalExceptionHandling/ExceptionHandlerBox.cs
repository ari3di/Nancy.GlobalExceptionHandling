using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Nancy.Responses;

namespace Nancy.GlobalExceptionHandling
{
    /// <summary>
    ///     A wrapper around IDictionary<Type, Func
    ///     <Request, Response>
    ///         >
    ///         to manage the handlers better
    /// </summary>
    public class ExceptionHandlerBox
    {
        // Each handler would react to a single exception type;
        private readonly IDictionary<Type, Func<NancyContext, Exception, Response>> _exceptionHandlers;
        private readonly Func<NancyContext, Exception, Response> _notHandledExceptionHandler;

        /// <summary>
        ///     Fields for Exceptions not handled by the handlers
        /// </summary>
        private readonly Type _notHandledExceptionType = typeof (NotHandledException);

        // A checkpoint for _exceptionHandlers dictionary, so it can only be set once
        private bool _handlersSet;
        // A checkpoint for NotHandledException handler, so it can be overriden only once
        private bool _notHandledExceptionHandlerSet;

        public ExceptionHandlerBox()
        {
            _notHandledExceptionHandler = (ctx, ex) => new NegotiatedResponse("Something went wrong")
                .WithStatusCode(HttpStatusCode.InternalServerError);

            _exceptionHandlers = new ConcurrentDictionary<Type, Func<NancyContext, Exception, Response>>();

            _exceptionHandlers.Add(_notHandledExceptionType, _notHandledExceptionHandler);
        }

        public void OverrideNothandledExceptionHandler(Func<NancyContext, Exception, Response> handler)
        {
            // Throwing an exception if NothandledExceptionHandler was overriden before
            if (_notHandledExceptionHandlerSet)
            {
                throw new Exception("Not Handled Exception handler was already set once");
            }

            if (_exceptionHandlers.TryGetValue(_notHandledExceptionType, out handler))
            {
                _exceptionHandlers[_notHandledExceptionType] = handler;
            }

            _notHandledExceptionHandlerSet = true; // Saving the checkpoint
        }

        public void Set(IDictionary<Type, Func<NancyContext, Exception, Response>> exceptionHandlers)
        {
            // Throwing an exception if Handlers were set before
            if (_handlersSet)
            {
                throw new Exception("ExceptionHandlerBox was already set once");
            }

            // NotHandledException should be overriden Seperatly
            // By OverrideNothandledExceptionHandler method
            if (exceptionHandlers.Keys.Any(type => type == typeof (NotHandledException)))
            {
                throw new Exception("NotHandledException handler should only be overriden with the method" +
                                    " GlobalExceptionHandler.Handlers.OverrideNothandledExceptionHandler");
            }

            foreach (var exceptionHandler in exceptionHandlers)
            {
                // Checking if the inputed Type extends the Extension Class
                var type = exceptionHandler.Key;
                if (!typeof (Exception).IsAssignableFrom(type))
                {
                    throw new Exception($"{type.Name} does not extend Exception");
                }

                _exceptionHandlers.Add(exceptionHandler);
            }

            _handlersSet = true; // Saving the checkpoint
        }

        /// <summary>
        ///     Returns the handler for given exception type
        ///     would return UnhandledExceptionHandler if
        ///     no handler was found for the requestion exception
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Func<NancyContext, Exception, Response> Get(Type type)
        {
            // Checking if the requested type extends Exception
            // Unnecessary and reduces performance
            /*if (!type.IsAssignableFrom(typeof(Exception)))
                {
                    throw new Exception($"{type.GetType().Name} does not extend Exception");
                }*/

            // Finding the handler for thrown exception
            Func<NancyContext, Exception, Response> handler;
            if (_exceptionHandlers.TryGetValue(type, out handler))
            {
                return _exceptionHandlers[type];
            }

            // Using the NotHandlerException handler if no handler was found
            // for the thrown exception
            if (_exceptionHandlers.TryGetValue(_notHandledExceptionType, out handler))
            {
                return _exceptionHandlers[_notHandledExceptionType];
            }

            // This should never happen but just in case
            throw new Exception("Blame author of Nancy.GlobalExceptionHandling library");
        }
    }
}