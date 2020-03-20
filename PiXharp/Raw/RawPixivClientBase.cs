using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PiXharp.Raw
{
    public abstract class RawPixivClientBase
    {
        public abstract bool Authenticated { get; }

        public abstract string? RefreshToken { get; }

        public abstract Task LoginAsync(string refreshToken);

        public abstract Task LoginAsync(string pixivID, string password);

        public abstract Task<IllustResponse> GetIllustDetailAsync(long id);

        public abstract Task<IllustsPageResponse> SearchAsync(string query);

        public abstract Task<Stream> DownloadIllustAsStreamAsync(Uri uri);

        public Task<Stream> DownloadIllustAsStreamAsync(string uri) => DownloadIllustAsStreamAsync(new Uri(uri));
    }
}
