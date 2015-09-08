using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Nancy.Responses;

namespace Nancy.GlobalExceptionHandling
{
    public static partial class GlobalExceptionHandler
    {
        public class ErrorBoks
        {
            private readonly IDictionary<Type, Func<Request, Response>> _dic;
            private bool _funcsSet = false;

            private readonly Type _notHandledType = typeof (NotHandledException);
            private bool _defultSet = false;
            public ErrorBoks()
            {
                _dic = new ConcurrentDictionary<Type, Func<Request, Response>>();

                _dic.Add(_notHandledType, (request) => new NegotiatedResponse("Something went wrong")
                    .WithStatusCode(HttpStatusCode.InternalServerError));
            }

            public void SetUnhandledExceptionFunc(Func<Request, Response> func)
            {
                if (_defultSet)
                {
                    throw new Exception("Not Handled Exception Func was already set once");
                }

                if (_dic.TryGetValue(_notHandledType, out func))
                {
                    _dic[_notHandledType] = func;
                }
                _defultSet = true;
            }
            public void SetErrors(IDictionary<Type, Func<Request, Response>> dic)
            {
                if (_funcsSet)
                {
                    throw new Exception("ErrorBoks was already set once");
                }

                foreach (var exEr in dic)
                {
                    var type = exEr.Key;

                    if (!type.IsAssignableFrom(typeof(Exception)))
                    {
                        throw new Exception($"{type.GetType().Name} does not extend Exception");
                    }

                    _dic.Add(exEr);
                }
                
                _funcsSet = true;
            }

            public Func<Request, Response> Get(Type type)
            {
                //Unnecessary and reduces performance
                /*if (!type.IsAssignableFrom(typeof(Exception)))
                {
                    throw new Exception($"{type.GetType().Name} does not extend Exception");
                }*/

                Func<Request,Response> func;
                if (_dic.TryGetValue(type, out func))
                {
                    return _dic[type];
                }

                if (_dic.TryGetValue(_notHandledType, out func))
                {
                    return _dic[_notHandledType];
                }

                throw new Exception("Author of Nancy.GlobalExceptionHandling Library sucks");
            }
        }
    }
}