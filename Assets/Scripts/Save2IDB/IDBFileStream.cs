using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Save2IDB
{
    internal class IDBFileStream : FileStream
    {
        [DllImport("__Internal")] private static extern void JS_FileSystem_Sync();

        public override bool IsAsync => false;

        public IDBFileStream(string path, FileMode mode, FileAccess access, FileShare share)
            : base(path, mode, access, share)
        {
        }

        private void Sync()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            JS_FileSystem_Sync();
#endif
        }

        public override void Flush()
        {
            base.Flush();
            Sync();
        }

        public override void Flush(bool flushToDisk)
        {
            base.Flush(flushToDisk);
            Sync();
        }

        protected override void Dispose(bool disposing)
        {
            Sync();
            base.Dispose(disposing);
        }

        /// <summary>
        /// IL2CPP では標準非同期処理がサポートされないため例外を投げるのみ
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        public sealed override System.IAsyncResult BeginRead(byte[] buffer, int offset, int count, System.AsyncCallback callback, object state)
            => throw new System.NotSupportedException(
                $"{nameof(IDBFileStream)} で {nameof(BeginRead)} を使用することはできません。");

        /// <summary>
        /// IL2CPP では標準非同期処理がサポートされないため例外を投げるのみ
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        public sealed override System.IAsyncResult BeginWrite(byte[] buffer, int offset, int count, System.AsyncCallback callback, object state)
            => throw new System.NotSupportedException(
                $"{nameof(IDBFileStream)} で {nameof(BeginWrite)} を使用することはできません。");

        /// <summary>
        /// IL2CPP では標準非同期処理がサポートされないため例外を投げるのみ
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        public sealed override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
            => throw new System.NotSupportedException(
                $"{nameof(IDBFileStream)} で {nameof(CopyToAsync)} を使用することはできません。");

        /// <summary>
        /// IL2CPP では標準非同期処理がサポートされないため例外を投げるのみ
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        public sealed override ValueTask DisposeAsync()
            => throw new System.NotSupportedException(
                $"{nameof(IDBFileStream)} で {nameof(DisposeAsync)} を使用することはできません。" +
                $"代わりに {nameof(IDBFileStream.DisposeAsync)} を使用してください。");

        /// <summary>
        /// IL2CPP では標準非同期処理がサポートされないため例外を投げるのみ
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        public sealed override int EndRead(System.IAsyncResult asyncResult)
            => throw new System.NotSupportedException(
                $"{nameof(IDBFileStream)} で {nameof(EndRead)} を使用することはできません。");

        /// <summary>
        /// IL2CPP では標準非同期処理がサポートされないため例外を投げるのみ
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        public sealed override void EndWrite(System.IAsyncResult asyncResult)
            => throw new System.NotSupportedException(
                $"{nameof(IDBFileStream)} で {nameof(EndWrite)} を使用することはできません。");

        /// <summary>
        /// IL2CPP では標準非同期処理がサポートされないため例外を投げるのみ
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        public sealed override Task FlushAsync(CancellationToken cancellationToken)
            => throw new System.NotSupportedException(
                $"{nameof(IDBFileStream)} で {nameof(FlushAsync)} を使用することはできません。" +
                $"代わりに {nameof(IDBFileStream.FlushAsync)} を使用してください。");

        /// <summary>
        /// IL2CPP では標準非同期処理がサポートされないため例外を投げるのみ
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        public sealed override ValueTask<int> ReadAsync(System.Memory<byte> buffer, CancellationToken cancellationToken = default)
            => throw new System.NotSupportedException(
                $"{nameof(IDBFileStream)} で {nameof(ReadAsync)} を使用することはできません。");

        /// <summary>
        /// IL2CPP では標準非同期処理がサポートされないため例外を投げるのみ
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        public sealed override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => throw new System.NotSupportedException(
                $"{nameof(IDBFileStream)} で {nameof(ReadAsync)} を使用することはできません。");

        /// <summary>
        /// IL2CPP では標準非同期処理がサポートされないため例外を投げるのみ
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        public sealed override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => throw new System.NotSupportedException(
                $"{nameof(IDBFileStream)} で {nameof(WriteAsync)} を使用することはできません。");

        /// <summary>
        /// IL2CPP では標準非同期処理がサポートされないため例外を投げるのみ
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        public sealed override ValueTask WriteAsync(System.ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
            => throw new System.NotSupportedException(
                $"{nameof(IDBFileStream)} で {nameof(WriteAsync)} を使用することはできません。");
    }
}
