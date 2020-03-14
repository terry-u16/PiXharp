using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PiXharp.Authentication
{
    internal class Token
    {
        internal string AccessToken { get; }

        internal string RefreshToken { get; }

        internal long ID { get; }

        internal Token(string accessToken, string refreshToken, long id)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ID = id;
        }
    }
}
