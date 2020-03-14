using System;
using System.Threading.Tasks;

namespace PiXharp
{
    public abstract class PixivClientBase
    {
        public abstract bool Authenticated { get; }

        public abstract Task LoginAsync(string pixivID, string password);
    }
}
