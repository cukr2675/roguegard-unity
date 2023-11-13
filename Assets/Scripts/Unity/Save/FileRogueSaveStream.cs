using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using Save2IDB;
using System.Threading.Tasks;

namespace RoguegardUnity
{
    public class FileRogueSaveStream : RogueSaveStream
    {
        private readonly FileStream baseStream;

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => true;
        public override long Length => baseStream.Length;

        public override long Position
        {
            get => baseStream.Position;
            set => baseStream.Position = value;
        }

        public FileRogueSaveStream(string path)
            : base(path)
        {
            baseStream = File.Create(path);
        }

        public override void Flush()
        {
            baseStream.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            baseStream.Dispose();
            base.Dispose(disposing);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return baseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin loc)
        {
            return baseStream.Seek(offset, loc);
        }

        public override void SetLength(long value)
        {
            baseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            baseStream.Write(buffer, offset, count);
        }
    }
}
