using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PiXharp
{
    public class ImageStream : Stream
    {
        public string FileName { get; }

        private Stream? _innerStream;

        internal ImageStream(Stream stream, string fileName) : base()
        {
            _innerStream = stream;
            FileName = fileName;
        }

        public override bool CanRead => _innerStream?.CanRead ?? throw new ObjectDisposedException(nameof(ImageStream));

        public override bool CanSeek => _innerStream?.CanSeek ?? throw new ObjectDisposedException(nameof(ImageStream));

        public override bool CanWrite => _innerStream?.CanWrite ?? throw new ObjectDisposedException(nameof(ImageStream));

        public override long Length => _innerStream?.Length ?? throw new ObjectDisposedException(nameof(ImageStream));

        public override long Position
        {
            get => _innerStream?.Position ?? throw new ObjectDisposedException(nameof(ImageStream));
            set
            {
                if (_innerStream != null)
                {
                    _innerStream.Position = value;
                }
                else
                {
                    throw new ObjectDisposedException(nameof(ImageStream));
                }
            }
        }

        public override void Flush()
        {
            if (_innerStream != null)
            {
                _innerStream.Flush();
            }
            else
            {
                throw new ObjectDisposedException(nameof(ImageStream));
            }
        }

        public override int Read(byte[] buffer, int offset, int count) => _innerStream?.Read(buffer, offset, count) ?? throw new ObjectDisposedException(nameof(ImageStream));

        public override long Seek(long offset, SeekOrigin origin) => _innerStream?.Seek(offset, origin) ?? throw new ObjectDisposedException(nameof(ImageStream));

        public override void SetLength(long value)
        {
            if (_innerStream != null)
            {
                _innerStream.SetLength(value);
            }
            else
            {
                throw new ObjectDisposedException(nameof(ImageStream));
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_innerStream != null)
            {
                _innerStream.Write(buffer, offset, count);
            }
            else
            {
                throw new ObjectDisposedException(nameof(ImageStream));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _innerStream?.Dispose();
                _innerStream = null;    // prevent memory leak
            }
            base.Dispose(disposing);
        }

        private void ThrowIfDisposed()
        {
            if (_innerStream == null)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}
