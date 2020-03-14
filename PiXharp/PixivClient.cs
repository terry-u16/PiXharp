﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PiXharp.Authentication;
using PiXharp.Exceptions;
using PiXharp.RawObjects;

namespace PiXharp
{
    public class PixivClient : PixivClientBase, IDisposable
    {
        private readonly HttpClient _innerClient;
        private Token? _token;

        private const string clientID = "MOBrBDS8blbauoSck0ZfDbtuzpyT";
        private const string clientSecret = "lsACyCD94FhDUtGTXi3QzcFE2uU1hqtDaKeqrdwj";
        private const string hashSecret = "28c1fdd170a5204386cb1313c7077b34f83e4aaf4aa829ce78c231e05b0bae2c";

        public PixivClient()
        {
            _innerClient = new HttpClient();
            _innerClient.DefaultRequestHeaders.Add("host", "app-api.pixiv.net");
            _innerClient.DefaultRequestHeaders.Add("App-OS", "ios");
            _innerClient.DefaultRequestHeaders.Add("App-OS-Version", "12.2");
            _innerClient.DefaultRequestHeaders.Add("App-Version", "7.6.2");
            _innerClient.DefaultRequestHeaders.Add("User-Agent", "PixivIOSApp/7.6.2 (iOS 12.2; iPhone9,1)");
            _innerClient.BaseAddress = new Uri("https://app-api.pixiv.net");
        }

        public override async Task LoginAsync(string pixivID, string password)
        {
            _token = await PixivAuthenticator.AuthenticateAsync(pixivID, password, clientID, clientSecret, hashSecret);
            _innerClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token.AccessToken}");
        }

        public override bool Authenticated => _token != null;

        public override async Task<IllustsPage> SearchAsync(string query)
        {
            if (!Authenticated)
            {
                throw new PixivNotAuthenticatedException("Token is null. You must login before search.");
            }

            const string relativeUrl = "/v1/search/illust";
            var parameters = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "word", query },
                // TODO: ちゃんと実装する
                { "search_target", "partial_match_for_tags" },
                { "sort", "date_desc" },
                { "filter", "for_ios" }
            });

            var response = await _innerClient.GetAsync($"{relativeUrl}?{await parameters.ReadAsStringAsync()}");
            var json = await response.Content.ReadAsStringAsync();
            var illustsPage = JsonSerializer.Deserialize<IllustsPage>(json);
            return illustsPage;
        }

        #region IDisposable Support
        // Dispose pattern
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _innerClient.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}