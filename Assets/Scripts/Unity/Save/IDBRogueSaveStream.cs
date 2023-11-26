using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using Save2IDB;

namespace RoguegardUnity
{
    public class IDBRogueSaveStream : RogueSaveStream
    {
        private readonly MemoryStream baseStream;

        public override bool CanRead => baseStream.CanRead;
        public override bool CanSeek => baseStream.CanSeek;
        public override bool CanWrite => baseStream.CanWrite;
        public override long Length => baseStream.Length;

        public override long Position
        {
            get => baseStream.Position;
            set => baseStream.Position = value;
        }

        public IDBRogueSaveStream(string path)
            : base(path)
        {
            baseStream = new MemoryStream();
        }

        public override void Save(System.Action callback)
        {
            FadeCanvas.StartCanvasCoroutine(SaveCoroutine(callback));
        }

        private IEnumerator SaveCoroutine(System.Action callback)
        {
            var handle = IDBFile.DirectWriteAsync(Path, baseStream.ToArray());
            yield return handle;

            switch (handle.Status)
            {
                case IDBOperationStatus.Succeeded:
                    Status = RogueSaveStreamStatus.Succeeded;
                    callback();
                    break;
                case IDBOperationStatus.Failed:
                    Status = RogueSaveStreamStatus.Failed;
                    break;
                default:
                    Status = RogueSaveStreamStatus.None;
                    break;
            }
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
