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
            using var stream = new FileStream("user.json", FileMode.Open, FileAccess.Read);
            var profile = await JsonSerializer.DeserializeAsync<UserAuthenticationProfile>(stream);
            using var client = new PixivClient();

            await client.LoginAsync(profile.PixivID ?? "", profile.Password ?? "");

            Assert.True(client.Authenticated);
        }
    }
}
