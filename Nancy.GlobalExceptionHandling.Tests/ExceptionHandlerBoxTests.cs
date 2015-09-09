using System;
using System.Collections.Generic;
using Nancy.GlobalExceptionHandling;
using Nancy.Responses;
using Xunit;

namespace Nancy.GlobalExceptionHandling.Tests
{
    public class ExceptionHandlerBoxTests
    {
        [Fact]
        public void Cant_Override_UnhandledException_Handler_MultipleTimes()
        {
            var box = new ExceptionHandlerBox();

            box.OverrideNothandledExceptionHandler((ctx, ex) => null);

            var exp = Assert.Throws<Exception>(
                () => box.OverrideNothandledExceptionHandler((ctx, ex) => null));

            Assert.Equal("Not Handled Exception handler was already set once", exp.Message);
        }

        [Fact]
        public void Cant_Set_ExceptionHandlers_Twice()
        {
            var box = new ExceptionHandlerBox();

            var handlers = new Dictionary<Type, Func<NancyContext, Exception, Response>>();
            handlers.Add(typeof(SomeException), (ctx, ex) => null);
            handlers.Add(typeof(OtherException), (ctx, ex) => null);

            box.Set(handlers);

            var exp = Assert.Throws<Exception>( 
                () => box.Set(handlers));

            Assert.Equal("ExceptionHandlerBox was already set once", exp.Message);
        }

        [Fact]
        public void Cant_Set_NothandledException_Handler_With_Set_Method()
        {
            var box = new ExceptionHandlerBox();

            var handlers = new Dictionary<Type, Func<NancyContext, Exception, Response>>();
            handlers.Add(typeof(NotHandledException), (ctx, ex) => null);

            var exp = Assert.Throws<Exception>(
                () => box.Set(handlers));

            Assert.Equal("NotHandledException handler should only be overriden with the method" +
                         " GlobalExceptionHandler.Handlers.OverrideNothandledExceptionHandler", exp.Message);
        }

        [Fact]
        public void Handlers_Key_Must_Extend_Exception()
        {
            var box = new ExceptionHandlerBox();

            var handlers = new Dictionary<Type, Func<NancyContext, Exception, Response>>();
            handlers.Add(typeof(NancyModule), (ctx, ex) => null);

            var exp = Assert.Throws<Exception>(
                () => box.Set(handlers));

            Assert.Equal($"{typeof(NancyModule).Name} does not extend Exception", exp.Message);
        }

    }
}
