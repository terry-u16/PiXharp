using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PiXharp.Test
{
    public class UserAuthenticationProfile
    {
        [JsonPropertyName("pixiv_id")]
        public string? PixivID { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }
}
