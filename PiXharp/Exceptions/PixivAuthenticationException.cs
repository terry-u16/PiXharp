using System;
using System.Collections.Generic;
using System.Text;

namespace PiXharp.Exceptions
{
    public class PixivAuthenticationException : PixivException
    {
        public PixivAuthenticationException(string message) : base(message)
        {
        }

        public PixivAuthenticationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
