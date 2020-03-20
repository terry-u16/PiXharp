using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PiXharp.Raw;

namespace PiXharp
{
    public class PixivClient : PixivClientBase
    {
        private RawPixivClientBase _rawClient = new RawPixivClient();

        public override bool Authenticated => _rawClient.Authenticated;

        public override string? RefreshToken => _rawClient.RefreshToken;

        public PixivClient()
        {
        }

        public override Task LoginAsync(string refreshToken) => _rawClient.LoginAsync(refreshToken);

        public override async Task LoginAsync(string pixivID, string password) => await _rawClient.LoginAsync(pixivID, password);
    }
}
