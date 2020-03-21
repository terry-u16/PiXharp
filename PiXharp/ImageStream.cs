using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PiXharp
{
    public class ImageStream : MemoryStream
    {
        public string FileName { get; }

        internal ImageStream(Stream stream, string fileName) : base()
        {
            stream.CopyTo(this);
            FileName = fileName;
            Position = 0;
        }
    }
}
