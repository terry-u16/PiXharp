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
            var result = await client.SearchAsync("�����q�T");

            Assert.NotNull(result.Illusts);
            Assert.NotEmpty(result.Illusts);
            Assert.True(result.Illusts.All(i => !string.IsNullOrEmpty(i.Title)));
            Assert.True(Uri.TryCreate(result.NextUrl, UriKind.Absolute, out _));
        }

        [Fact]
        public async Task SearchIllustsWithMultipleQueriesAsyncTest()
        {
            var client = await GetAuthenticatedClient();
            var result = await client.SearchAsync("�����q�T �ۓo�S��");

            Assert.NotNull(result.Illusts);
            Assert.NotEmpty(result.Illusts);
            Assert.True(result.Illusts.All(i => !string.IsNullOrEmpty(i.Title)));
            Assert.True(Uri.TryCreate(result.NextUrl, UriKind.Absolute, out _));
        }

        [Fact]
        public async Task SearchRatedIllustsAsyncTest()
        {
            var client = await GetAuthenticatedClient();
            var result = await client.SearchAsync("�����q�T R-18");

            Assert.NotNull(result.Illusts);
            Assert.NotEmpty(result.Illusts);
            Assert.True(result.Illusts.All(i => !string.IsNullOrEmpty(i.Title)));
            Assert.True(Uri.TryCreate(result.NextUrl, UriKind.Absolute, out _));
        }

        [Fact]
        public async Task DownloadSingleIllustAsStreamByUriAsyncTest()
        {
            var client = await GetAuthenticatedClient();
            var uri = new Uri("https://i.pximg.net/img-original/img/2020/03/19/19/20/13/80221680_p0.jpg");
            var sha256 = System.Security.Cryptography.SHA256.Create();

            using var stream = await client.DownloadIllustAsStreamAsync(uri);
            var hash = BitConverter.ToString(sha256.ComputeHash(stream)).Replace("-", "");

            Assert.Equal("F5FC970E3285FF8D13157B0AFABCFB74D751B78DAD24198D25C1AD6B883FF21E", hash);
        }
    }
}
