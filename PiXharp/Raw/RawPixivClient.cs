using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PiXharp.Authentication;
using PiXharp.Exceptions;

namespace PiXharp.Raw
{
    public class RawPixivClient : RawPixivClientBase
    {
        private readonly HttpClient _innerClient;
        private Token? _token;

        private const string clientID = "MOBrBDS8blbauoSck0ZfDbtuzpyT";
        private const string clientSecret = "lsACyCD94FhDUtGTXi3QzcFE2uU1hqtDaKeqrdwj";
        private const string hashSecret = "28c1fdd170a5204386cb1313c7077b34f83e4aaf4aa829ce78c231e05b0bae2c";

        public override string? RefreshToken => _token?.RefreshToken;

        public RawPixivClient()
        {
            var handler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                {
                    switch (sslPolicyErrors)
                    {
                        case System.Net.Security.SslPolicyErrors.None:
                            return true;
                        case System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch:
                            return sender.RequestUri.Host == "i.pximg.net";     // Ignore
                        default:
                            return false;
                    }
                }
            };

            _innerClient = new HttpClient(handler);
            _innerClient.DefaultRequestHeaders.Add("host", "app-api.pixiv.net");
            _innerClient.DefaultRequestHeaders.Add("App-OS", "ios");
            _innerClient.DefaultRequestHeaders.Add("App-OS-Version", "12.2");
            _innerClient.DefaultRequestHeaders.Add("App-Version", "7.6.2");
            _innerClient.DefaultRequestHeaders.Add("User-Agent", "PixivIOSApp/7.6.2 (iOS 12.2; iPhone9,1)");
            _innerClient.BaseAddress = new Uri("https://app-api.pixiv.net");
        }

        public override async Task LoginAsync(string refreshToken)
        {
            _token = await PixivAuthenticator.AuthenticateAsync(refreshToken, clientID, clientSecret, hashSecret);
            _innerClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token.AccessToken}");
        }

        public override async Task LoginAsync(string pixivID, string password)
        {
            _token = await PixivAuthenticator.AuthenticateAsync(pixivID, password, clientID, clientSecret, hashSecret);
            _innerClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token.AccessToken}");
        }


        public override bool Authenticated => _token != null;

        public override async Task<IllustResponse> GetIllustDetailAsync(long id)
        {
            if (!Authenticated)
            {
                throw new PixivNotAuthenticatedException("Token is null. You must login before search.");
            }

            const string relativeUrl = "/v1/illust/detail";
            var parameters = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "illust_id", id.ToString() }
            });

            var response = await _innerClient.GetAsync($"{relativeUrl}?{await parameters.ReadAsStringAsync()}");
            var json = await response.Content.ReadAsStringAsync();
            var illust = JsonSerializer.Deserialize<IllustContainerResponse>(json).Illust ?? throw new PixivNotFoundException($"Illust id {id} is not found. Make sure illust id is valid.");
            return illust;
        }

        public override async Task<IllustsPageResponse> SearchAsync(string query)
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
            var illustsPage = JsonSerializer.Deserialize<IllustsPageResponse>(json);
            return illustsPage;
        }

        public override async Task<Stream> DownloadIllustAsStreamAsync(Uri uri)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                request.Headers.Add("Referer", "https://app-api.pixiv.net/");
                var response = await _innerClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStreamAsync();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new PixivNotFoundException("Image not found. Make sure the uri is valid.");
                }
                else
                {
                    throw new PixivException($"Http error occured. Status code: {response.StatusCode} {response.ReasonPhrase}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new PixivException("Http error occured. For more information, please check inner exceptions.", ex);
            }
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
