using System;
 
namespace ServiceProxy.Exceptions
{   
    class ServiceNotFoundException : Exception
    {
        public int StatusCode { get; set; }

        public ServiceNotFoundException(string message) : base(message)
        { }

        public ServiceNotFoundException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
