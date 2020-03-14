using System;
using System.Collections.Generic;
using System.Text;

namespace PiXharp.Exceptions
{
    public class PixivNotAuthenticatedException : PixivException
    {
        public PixivNotAuthenticatedException(string message) : base(message)
        {
        }

        public PixivNotAuthenticatedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
