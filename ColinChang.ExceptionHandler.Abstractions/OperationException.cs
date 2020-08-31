using System;

namespace ColinChang.ExceptionHandler.Abstractions
{
    public class OperationException : Exception
    {
        /// <summary>
        /// default custom friendly exception message that can be seen by client users
        /// </summary>
        public static string DefaultMessage { get; set; } =
            "error occured when execute the current request. please try again later or contact the administrator.";

        public OperationException() : base(DefaultMessage)
        {
        }


        public OperationException(string message) : base(string.IsNullOrWhiteSpace(message) ? DefaultMessage : message)
        {
        }

        public OperationException(string message, Exception innerException)
            : base(string.IsNullOrWhiteSpace(message) ? DefaultMessage : message, innerException)
        {
        }
    }
}