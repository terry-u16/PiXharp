using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PiXharp.Authentication;

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
            _innerClient.DefaultRequestHeaders.Add("User-Agent", "PixivAndroidApp/5.0.64 (Android 6.0)");
        }

        public override async Task LoginAsync(string pixivID, string password) => _token = await PixivAuthenticator.AuthenticateAsync(pixivID, password, clientID, clientSecret, hashSecret);

        public override bool Authenticated => _token != null;

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
