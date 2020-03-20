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
    }
}
