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

        private async Task<PixivClient> GetAuthenticatedClient()
        {
            var refreshToken = await PixivClientTestUtility.LoadTokenAsync("refresh_token.txt");
            var client = new PixivClient();
            await client.LoginAsync(refreshToken);

            return client;
        }
    }
}
