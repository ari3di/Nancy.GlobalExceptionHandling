using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Nancy.Responses;

namespace Nancy.GlobalExceptionHandling
{
    public static partial class GlobalExceptionHandler
    {
        /// <summary>
        /// A wrapper around IDictionary<Type, Func<Request, Response>>
        /// to manage the handlers better
        /// </summary>
        public class ExceptionHandlerBox
        {
            // Each handler would react to a single exception type;
            private readonly IDictionary<Type, Func<Request, Response>> _exceptionHandlers;

            // A checkpoint for _exceptionHandlers dictionary, so it can only be set once
            private bool _handlersSet = false;  


            /// <summary>
            /// Fields for Exceptions not handled by the handlers
            /// </summary>
            private readonly Type _notHandledExceptionType = typeof (NotHandledException);
            private readonly Func<Request, Response> _notHandledExceptionHandler;

            // A checkpoint for NotHandledException handler, so it can be overriden only once
            private bool _notHandledExceptionHandlerSet = false;


            public ExceptionHandlerBox()
            {
                _notHandledExceptionHandler = (Request) => new NegotiatedResponse("Something went wrong")
                    .WithStatusCode(HttpStatusCode.InternalServerError);

                _exceptionHandlers = new ConcurrentDictionary<Type, Func<Request, Response>>();

                _exceptionHandlers.Add(_notHandledExceptionType, _notHandledExceptionHandler);
            }

            public void OverrideUnhandledExceptionHandler(Func<Request, Response> handler)
            {
                // Throwing an exception if UnhandledExceptionHandler was overriden before
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

            public void SetExceptionHandlers(IDictionary<Type, Func<Request, Response>> exceptionHandlers)
            {
                // Throwing an exception if Handlers were set before
                if (_handlersSet)
                {
                    throw new Exception("ExceptionHandlerBox was already set once");
                }

                // NotHandledException should be overriden Seperatly
                // By OverrideUnhandledExceptionHandler method
                if (exceptionHandlers.Keys.Any(type => type == typeof (NotHandledException)))
                {
                    throw new Exception("NotHandledException handler should only be overriden with the method" +
                                        "GlobalExceptionHandler.ExceptionHandlers.OverrideUnhandledExceptionHandler");
                }

                foreach (var exceptionHandler in exceptionHandlers)
                {
                    // Checking if the inputed Type extends the Extension Class
                    var exception = exceptionHandler.Key;
                    if (!exception.IsAssignableFrom(typeof(Exception)))
                    {
                        throw new Exception($"{exception.GetType().Name} does not extend Exception");
                    }

                    _exceptionHandlers.Add(exceptionHandler);
                }
                
                _handlersSet = true; // Saving the checkpoint
            }

            /// <summary>
            /// Returns the handler for given exception type
            /// would return UnhandledExceptionHandler if 
            /// no handler was found for the requestion exception
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public Func<Request, Response> GetHandler(Type type)
            {
                // Checking if the requested type extends Exception
                // Unnecessary and reduces performance
                /*if (!type.IsAssignableFrom(typeof(Exception)))
                {
                    throw new Exception($"{type.GetType().Name} does not extend Exception");
                }*/

                // Finding the handler for thrown exception
                Func<Request,Response> handler;
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
}