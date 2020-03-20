using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PiXharp.Test
{
    internal static class PixivClientTestUtility
    {
        internal static async Task<UserAuthenticationProfile> GetProfileAsync(string jsonFileName)
        {
            using var stream = new FileStream(jsonFileName, FileMode.Open, FileAccess.Read);
            var profile = await JsonSerializer.DeserializeAsync<UserAuthenticationProfile>(stream);
            return profile;
        }

        internal static async Task<string> LoadTokenAsync(string tokenFileName)
        {
            using var stream = new StreamReader(tokenFileName, Encoding.UTF8);
            return await stream.ReadToEndAsync();
        }
    }
}
