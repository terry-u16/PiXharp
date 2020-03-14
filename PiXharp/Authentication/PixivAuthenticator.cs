﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PiXharp.Exceptions;

namespace PiXharp.Authentication
{
    internal static class PixivAuthenticator
    {
        private readonly static HttpClient _innerClient = new HttpClient();

        internal static async Task<Token> AuthenticateAsync(string pixivID, string password, string clientID, string clientSecret, string hashSecret)
        {
            var localTime = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz");
            using var md5 = MD5.Create();
            var clientHash = GetMD5HashString(localTime + hashSecret, Encoding.UTF8);

            var request = new HttpRequestMessage(HttpMethod.Post, @"https://oauth.secure.pixiv.net/auth/token");
            request.Headers.Add("User-Agent", "PixivAndroidApp/5.0.64 (Android 6.0)");
            request.Headers.Add("X-Client-Time", localTime);
            request.Headers.Add("X-Client-Hash", clientHash);
            request.Headers.Add("host", "oauth.secure.pixiv.net");

            var parameters = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "get_secure_url", "1" },
                { "client_id", clientID },
                { "client_secret", clientSecret },
                { "grant_type", "password" },
                { "username", pixivID },
                { "password", password }
            });
            request.Content = parameters;
            var response = await _innerClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var authenticationResponse = (await JsonSerializer.DeserializeAsync<AuthenticationResponse>(await response.Content.ReadAsStreamAsync())).Response;
                return new Token(authenticationResponse.AccessToken, authenticationResponse.RefreshToken, long.Parse(authenticationResponse.User.ID));
            }
            else
            {
                throw new PixivAuthenticationException("Failed to authenticate. Make sure ID and password are correct.");
            }
        }

        private static string GetMD5HashString(string value, Encoding encoding)
        {
            using var md5 = MD5.Create();
            var bytes = encoding.GetBytes(value);
            var md5Bytes = md5.ComputeHash(bytes);
            var resultBuilder = new StringBuilder();
            foreach (var b in md5Bytes)
            {
                resultBuilder.Append(b.ToString("x2"));
            }

            return resultBuilder.ToString();
        }

        
    }
}