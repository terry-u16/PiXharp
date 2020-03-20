using System;
using Xunit;
using PiXharp;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Linq;
using PiXharp.Exceptions;
using System.Text;

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

        [Fact]
        public async Task GetRefreshTokenTest()
        {
            var client = await GetAuthenticatedClient();
            Assert.NotNull(client.RefreshToken);
        }

        [Fact]
        public async Task LoginByRefreshTokenTest()
        {
            var client = new PixivClient();
            
            var refreshToken = await LoadTokenAsync();
            await client.LoginAsync(refreshToken);

            Assert.True(client.Authenticated);
        }

        private async Task<string> LoadTokenAsync()
        {
            using var stream = new StreamReader("refresh_token.txt", Encoding.UTF8);
            return await stream.ReadToEndAsync();
        }

        private async Task<PixivClientBase> GetAuthenticatedClient()
        {
            var refreshToken = await LoadTokenAsync();
            PixivClientBase client = new PixivClient();
            await client.LoginAsync(refreshToken);

            return client;
        }

        [Fact]
        public async Task GetIllustDetailAsyncTest()
        {
            const long id = 80221680L;
            var client = await GetAuthenticatedClient();
            var result = await client.GetIllustDetailAsync(id);

            Assert.Equal(id, result.ID);
            Assert.Equal("ÅuÇ‡Ç§í©ÅEÅEÅHÅv", result.Title);
        }

        [Fact]
        public async Task GetIllustDetailOfInvalidIdThrowsExceptionTest()
        {
            const long id = 0L;
            var client = await GetAuthenticatedClient();

            await Assert.ThrowsAsync<PixivNotFoundException>(async () => await client.GetIllustDetailAsync(id));
        }

        [Fact]
        public async Task GetIllustWithoutAuthenticationThrowsExceptionTest()
        {
            const long id = 0L;
            var client = new PixivClient();
            await Assert.ThrowsAsync<PixivNotAuthenticatedException>(async () => await client.GetIllustDetailAsync(id));
        }

        [Fact]
        public async Task SearchIllustsAsyncTest()
        {
            var client = await GetAuthenticatedClient();
            var result = await client.SearchAsync("çÅïóíqîT");

            Assert.NotNull(result.Illusts);
            Assert.NotEmpty(result.Illusts);
            Assert.True(result.Illusts.All(i => !string.IsNullOrEmpty(i.Title)));
            Assert.True(Uri.TryCreate(result.NextUrl, UriKind.Absolute, out _));
        }

        [Fact]
        public async Task SearchIllustsWithMultipleQueriesAsyncTest()
        {
            var client = await GetAuthenticatedClient();
            var result = await client.SearchAsync("çÅïóíqîT ï€ìoêSà§");

            Assert.NotNull(result.Illusts);
            Assert.NotEmpty(result.Illusts);
            Assert.True(result.Illusts.All(i => !string.IsNullOrEmpty(i.Title)));
            Assert.True(Uri.TryCreate(result.NextUrl, UriKind.Absolute, out _));
        }

        [Fact]
        public async Task SearchRatedIllustsAsyncTest()
        {
            var client = await GetAuthenticatedClient();
            var result = await client.SearchAsync("çÅïóíqîT R-18");

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

            using var stream = await client.DownloadIllustAsStreamAsync(uri);
            var hash = GetSha256Hash(stream);

            Assert.Equal("F5FC970E3285FF8D13157B0AFABCFB74D751B78DAD24198D25C1AD6B883FF21E", hash);
        }

        [Fact]
        public async Task DownloadSingleIllustAsStreamByStringAsyncTest()
        {
            var client = await GetAuthenticatedClient();
            var uri = "https://i.pximg.net/img-original/img/2020/03/19/19/20/13/80221680_p0.jpg";

            using var stream = await client.DownloadIllustAsStreamAsync(uri);
            var hash = GetSha256Hash(stream);

            Assert.Equal("F5FC970E3285FF8D13157B0AFABCFB74D751B78DAD24198D25C1AD6B883FF21E", hash);
        }

        private string GetSha256Hash(Stream stream)
        {
            var sha256 = System.Security.Cryptography.SHA256.Create();
            var hash = BitConverter.ToString(sha256.ComputeHash(stream)).Replace("-", "");
            return hash;
        }

        [Fact]
        public async Task DownloadSingleIllustAsStreamByInvalidUriThrowsPixivNotFoundExceptionTest()
        {
            var client = await GetAuthenticatedClient();
            var uri = new Uri("https://i.pximg.net/img-original/img/2020/03/19/19/20/13/xxxxxxxx_p0.jpg");

            await Assert.ThrowsAsync<PixivNotFoundException>(async () => await client.DownloadIllustAsStreamAsync(uri));
        }
    }
}
