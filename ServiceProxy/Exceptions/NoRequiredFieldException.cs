using System;

namespace ServiceProxy.Exceptions
{
    class NoRequiredFieldException : Exception
    {
        public string MissedField { get; set; }

        public NoRequiredFieldException(string message) : base(message)
        { }

        public NoRequiredFieldException(string message, string field) : base(message)
        {
            MissedField = field;
        }
    }
}
