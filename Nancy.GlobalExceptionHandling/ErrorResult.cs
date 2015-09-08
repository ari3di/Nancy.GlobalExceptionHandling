using System;

namespace Nancy.GlobalExceptionHandling
{
    public class ErrorResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
}
