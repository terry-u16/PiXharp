using System;
using System.Threading.Tasks;
using PiXharp.RawObjects;

namespace PiXharp
{
    public abstract class PixivClientBase
    {
        public abstract bool Authenticated { get; }

        public abstract Task LoginAsync(string pixivID, string password);

        public abstract Task<IllustsPage> SearchAsync(string query);
    }
}
