using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PiXharp
{
    public abstract class PixivClientBase : IDisposable
    {
        public abstract bool Authenticated { get; }

        public abstract string? RefreshToken { get; }

        public abstract Task LoginAsync(string refreshToken);

        public abstract Task LoginAsync(string pixivID, string password);


        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            // Do nothing
        }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
        }
        #endregion

    }
}
