using System;
using Xunit;
using PiXharp;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Linq;
using PiXharp.Exceptions;
using System.Text;
using PiXharp.Raw;

namespace PiXharp.Test
{
    public class RawPixivClientTest
    {
        [Fact]
        public async Task LoginTest()
        {
            var profile = await PixivClientTestUtility.GetProfileAsync("user.json");
            using var client = new RawPixivClient();

            await client.LoginAsync(profile.PixivID ?? "", profile.Password ?? "");

            Assert.True(client.Authenticated);
        }

        [Fact]
        public async Task LoginFailureThrowsExceptionTest()
        {
            using var client = new RawPixivClient();

            await Assert.ThrowsAsync<PixivAuthenticationException>(() => client.LoginAsync("", ""));
        }

        [Fact]
        public async Task GetRefreshTokenTest()
        {
            using var client = await GetAuthenticatedClient();
            Assert.NotNull(client.RefreshToken);
        }

        [Fact]
        public async Task LoginByRefreshTokenTest()
        {
            using var client = new RawPixivClient();
            
            var refreshToken = await PixivClientTestUtility.LoadTokenAsync("refresh_token.txt");
            await client.LoginAsync(refreshToken);

            Assert.True(client.Authenticated);
        }

        private async Task<RawPixivClient> GetAuthenticatedClient()
        {
            var refreshToken = await PixivClientTestUtility.LoadTokenAsync("refresh_token.txt");
            var client = new RawPixivClient();
            await client.LoginAsync(refreshToken);

            return client;
        }

        [Fact]
        public async Task GetIllustDetailAsyncTest()
        {
            const long id = 80221680L;
            using var client = await GetAuthenticatedClient();
            var result = await client.GetIllustDetailAsync(id);

            Assert.Equal(id, result.ID);
            Assert.Equal("ÅuÇ‡Ç§í©ÅEÅEÅHÅv", result.Title);
        }

        [Fact]
        public async Task GetIllustDetailOfInvalidIdThrowsExceptionTest()
        {
            const long id = 0L;
            using var client = await GetAuthenticatedClient();

            await Assert.ThrowsAsync<PixivNotFoundException>(async () => await client.GetIllustDetailAsync(id));
        }

        [Fact]
        public async Task GetIllustWithoutAuthenticationThrowsExceptionTest()
        {
            const long id = 0L;
            using var client = new RawPixivClient();
            await Assert.ThrowsAsync<PixivNotAuthenticatedException>(async () => await client.GetIllustDetailAsync(id));
        }

        [Fact]
        public async Task SearchIllustsAsyncTest()
        {
            using var client = await GetAuthenticatedClient();
            var result = await client.SearchAsync("çÅïóíqîT");

            Assert.NotNull(result.Illusts);
            Assert.NotEmpty(result.Illusts);
            Assert.True(result.Illusts.All(i => !string.IsNullOrEmpty(i.Title)));
            Assert.True(Uri.TryCreate(result.NextUrl, UriKind.Absolute, out _));
        }

        [Fact]
        public async Task SearchIllustsWithMultipleQueriesAsyncTest()
        {
            using var client = await GetAuthenticatedClient();
            var result = await client.SearchAsync("çÅïóíqîT ï€ìoêSà§");

            Assert.NotNull(result.Illusts);
            Assert.NotEmpty(result.Illusts);
            Assert.True(result.Illusts.All(i => !string.IsNullOrEmpty(i.Title)));
            Assert.True(Uri.TryCreate(result.NextUrl, UriKind.Absolute, out _));
        }

        [Fact]
        public async Task SearchRatedIllustsAsyncTest()
        {
            using var client = await GetAuthenticatedClient();
            var result = await client.SearchAsync("çÅïóíqîT R-18");

            Assert.NotNull(result.Illusts);
            Assert.NotEmpty(result.Illusts);
            Assert.True(result.Illusts.All(i => !string.IsNullOrEmpty(i.Title)));
            Assert.True(Uri.TryCreate(result.NextUrl, UriKind.Absolute, out _));
        }

        [Fact]
        public async Task DownloadSingleIllustAsStreamByUriAsyncTest()
        {
            using var client = await GetAuthenticatedClient();
            var uri = new Uri("https://i.pximg.net/img-original/img/2020/03/19/19/20/13/80221680_p0.jpg");

            using var stream = await client.DownloadIllustAsStreamAsync(uri);
            var hash = PixivClientTestUtility.GetSha256Hash(stream);

            Assert.Equal("F5FC970E3285FF8D13157B0AFABCFB74D751B78DAD24198D25C1AD6B883FF21E", hash);
        }

        [Fact]
        public async Task DownloadSingleIllustAsStreamByStringAsyncTest()
        {
            using var client = await GetAuthenticatedClient();
            var uri = "https://i.pximg.net/img-original/img/2020/03/19/19/20/13/80221680_p0.jpg";

            using var stream = await client.DownloadIllustAsStreamAsync(uri);
            var hash = PixivClientTestUtility.GetSha256Hash(stream);

            Assert.Equal("F5FC970E3285FF8D13157B0AFABCFB74D751B78DAD24198D25C1AD6B883FF21E", hash);
        }


        [Fact]
        public async Task DownloadSingleIllustAsStreamByInvalidUriThrowsPixivNotFoundExceptionTest()
        {
            using var client = await GetAuthenticatedClient();
            var uri = new Uri("https://i.pximg.net/img-original/img/2020/03/19/19/20/13/xxxxxxxx_p0.jpg");

            await Assert.ThrowsAsync<PixivNotFoundException>(async () => await client.DownloadIllustAsStreamAsync(uri));
        }
    }
}
