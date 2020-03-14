using System;
using Xunit;
using PiXharp;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace PiXharp.Test
{
    public class PixivClientTest
    {
        [Fact]
        public async Task LoginTest()
        {
            using var stream = new FileStream("user.json", FileMode.Open, FileAccess.Read);
            var profile = await JsonSerializer.DeserializeAsync<UserAuthenticationProfile>(stream);
            PixivClientBase client = new PixivClient();

            await client.LoginAsync(profile.PixivID ?? "", profile.Password ?? "");

            Assert.True(client.Authenticated);
        }

        [Fact]
        public async Task LoginFailureThrowsExceptionTest()
        {
            PixivClientBase client = new PixivClient();

            await Assert.ThrowsAsync<Exceptions.PixivAuthenticationException>(() => client.LoginAsync("", ""));
        }

        private async Task<PixivClientBase> GetAuthenticatedClient()
        {
            using var stream = new FileStream("user.json", FileMode.Open, FileAccess.Read);
            var profile = await JsonSerializer.DeserializeAsync<UserAuthenticationProfile>(stream);
            PixivClientBase client = new PixivClient();
            await client.LoginAsync(profile.PixivID ?? "", profile.Password ?? "");

            return client;
        }

        [Fact]
        public async Task SearchIllustsAsyncTest()
        {
            var client = await GetAuthenticatedClient();
            var result = await client.SearchAsync("•—’q”T");

            Assert.NotNull(result.Illusts);
            Assert.NotEmpty(result.Illusts);
            Assert.True(result.Illusts.All(i => !string.IsNullOrEmpty(i.Title)));
            Assert.True(Uri.TryCreate(result.NextUrl, UriKind.Absolute, out _));
        }
    }
}
