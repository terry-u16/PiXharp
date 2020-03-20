using System;
using System.Collections.Generic;
using System.Text;

namespace PiXharp.Exceptions
{
    public class PixivNotFoundException : PixivException
    {
        public PixivNotFoundException(string message) : base(message)
        {
        }

        public PixivNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
