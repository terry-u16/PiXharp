﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PiXharp.RawObjects;

namespace PiXharp
{
    public abstract class PixivClientBase
    {
        public abstract bool Authenticated { get; }

        public abstract string? RefreshToken { get; }

        public abstract Task LoginAsync(string refreshToken);

        public abstract Task LoginAsync(string pixivID, string password);

        public abstract Task<Illust> GetIllustDetailAsync(long id);

        public abstract Task<IllustsPage> SearchAsync(string query);

        public abstract Task<Stream> DownloadIllustAsStreamAsync(Uri uri);

        public Task<Stream> DownloadIllustAsStreamAsync(string uri) => DownloadIllustAsStreamAsync(new Uri(uri));
    }
}
