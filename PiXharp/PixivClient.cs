using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PiXharp.Exceptions;
using PiXharp.Raw;

namespace PiXharp
{
    public class PixivClient : PixivClientBase
    {
        private RawPixivClient _rawClient = new RawPixivClient();

        public override bool Authenticated => _rawClient.Authenticated;

        public override string? RefreshToken => _rawClient.RefreshToken;

        public PixivClient()
        {
        }

        public override Task LoginAsync(string refreshToken) => _rawClient.LoginAsync(refreshToken);

        public override async Task LoginAsync(string pixivID, string password) => await _rawClient.LoginAsync(pixivID, password);

        public override async IAsyncEnumerable<Illust> SearchIllustsAsync(string query)
        {
            var response = await _rawClient.SearchAsync(query);

            if (response.Illusts != null)
            {
                foreach (var illust in response.Illusts)
                {
                    yield return new Illust(illust);
                }
            }
            else
            {
                throw new PixivException("Illusts are null.");
            }

            var nextUrl = response.NextUrl;

            while (nextUrl != null)
            {
                response = await _rawClient.SearchAsyncByNextUri(nextUrl);

                if (response.Illusts != null)
                {
                    foreach (var illust in response.Illusts)
                    {
                        yield return new Illust(illust);
                    }
                }
                else
                {
                    break;
                }

                nextUrl = response.NextUrl;
            }
        }
    }
}
