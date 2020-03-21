using System;
using Xunit;
using PiXharp;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Linq;
using PiXharp.Exceptions;

namespace PiXharp.Test
{
    public class PixivClientTest
    {
        [Fact]
        public async Task LoginAsyncTest()
        {
            var profile = await PixivClientTestUtility.GetProfileAsync("user.json");
            using var client = new PixivClient();

            await client.LoginAsync(profile.PixivID ?? "", profile.Password ?? "");

            Assert.True(client.Authenticated);
        }

        public async Task LoginFailureThrowsExceptionTest()
        {
            using var client = new PixivClient();

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
            using var client = new PixivClient();

            var refreshToken = await PixivClientTestUtility.LoadTokenAsync("refresh_token.txt");
            await client.LoginAsync(refreshToken);

            Assert.True(client.Authenticated);
        }

        [Fact]
        public async Task SearchIllustsAsyncTest()
        {
            using var client = await GetAuthenticatedClient();

            var illusts = client.SearchIllustsAsync("香風智乃")
                                      .Take(100);

            await foreach (var illust in illusts)
            {
                Assert.InRange(illust.ID, 0, long.MaxValue);
                Assert.False(string.IsNullOrEmpty(illust.Title));
                Assert.InRange(illust.CreateDate, new DateTimeOffset(2010, 1, 1, 0, 0, 0, TimeSpan.FromHours(9)), DateTimeOffset.Now);
                Assert.InRange(illust.PageCount, 1, int.MaxValue);
                Assert.InRange(illust.TotalBookmarks, 0, int.MaxValue);
                Assert.DoesNotContain(illust.Tags, string.IsNullOrEmpty);
            }

        }

        [Fact]
        public async Task GetIllustDetailAsyncTest()
        {
            const long id = 80221680L;
            using var client = await GetAuthenticatedClient();
            var result = await client.GetIllustDetailAsync(id);

            Assert.Equal(id, result.ID);
            Assert.Equal("「もう朝・・？」", result.Title);
        }

        [Fact]
        public async Task GetImageFileNameTest()
        {
            const long id = 80221680L;
            using var client = await GetAuthenticatedClient();
            var result = await client.GetIllustDetailAsync(id);
            
            Assert.Equal($"{id}_p0_square1200.jpg", result.GetFileName(0, ImageSize.SquareMedium));
            Assert.Equal($"{id}_p0_master1200.jpg", result.GetFileName(0, ImageSize.Medium));
            Assert.Equal($"{id}_p0_master1200.jpg", result.GetFileName(0, ImageSize.Large));
            Assert.Equal($"{id}_p0.jpg", result.GetFileName(0, ImageSize.Original));
        }


        #region Private methods

        private async Task<PixivClient> GetAuthenticatedClient()
        {
            var refreshToken = await PixivClientTestUtility.LoadTokenAsync("refresh_token.txt");
            var client = new PixivClient();
            await client.LoginAsync(refreshToken);

            return client;
        }

        #endregion
    }
}
