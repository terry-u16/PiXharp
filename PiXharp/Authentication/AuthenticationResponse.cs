#nullable disable
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PiXharp.Authentication
{
    public class AuthenticationResponse
    {
        [JsonPropertyName("response")]
        public InnerAuthenticationResponse Response { get; set; }
    }

    public class InnerAuthenticationResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
        [JsonPropertyName("scope")]
        public string Scope { get; set; }
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonPropertyName("user")]
        public User User { get; set; }
        [JsonPropertyName("device_token")]
        public string DeviceToken { get; set; }
    }

    public class User
    {
        [JsonPropertyName("profile_image_urls")]
        public OAuthProfileImageUrls ProfileImageUrls { get; set; }
        [JsonPropertyName("id")]
        public string ID { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("account")]
        public string Account { get; set; }
        [JsonPropertyName("mail_address")]
        public string MailAddress { get; set; }
        [JsonPropertyName("is_premium")]
        public bool IsPremium { get; set; }
        [JsonPropertyName("x_restrict")]
        public int XRestrict { get; set; }
        [JsonPropertyName("is_mail_authorized")]
        public bool IsMailAuthorized { get; set; }
    }

    public class OAuthProfileImageUrls
    {
        [JsonPropertyName("px_16x16")] 
        public Uri Px16x16 { get; set; }
        [JsonPropertyName("px_50x50")] 
        public Uri Px50x50 { get; set; }
        [JsonPropertyName("px_170x170")] 
        public Uri Px170x170 { get; set; }
    }
}
