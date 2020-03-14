using System;
using System.Collections.Generic;
using System.Text;

namespace PiXharp.Exceptions
{
    public class PixivException : Exception
    {
        public PixivException(string message) : base(message)
        {
        }

        public PixivException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
