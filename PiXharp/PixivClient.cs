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

        public PixivClient()
        {
        }

        public override async Task LoginAsync(string pixivID, string password)
        {
            await _rawClient.LoginAsync(pixivID, password);
        }
    }
}
