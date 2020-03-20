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
