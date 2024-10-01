using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using AOT;

namespace Save2IDB
{
    public class IDBImporter : IDBOperationHandle<IDBImporter.ResultObj[]>, System.IDisposable
    {
        private string path;
        private bool isShown;
        private bool isDisposed;

        public bool Overwrite { get; set; }
        public string FilterAccept { get; set; }
        public bool Multiselect { get; set; }

        public override ResultObj[] Result => isDisposed ? throw new System.ObjectDisposedException(nameof(IDBImporter)) : base.Result;

        private delegate void ImportThenCallback(System.IntPtr importerPtr, string statsJson);
        private delegate void ReadThenCallback(System.IntPtr importerPtr);

        [DllImport("__Internal")]
        private static extern void Save2IDB_ImportToAsync(
            string path, bool overwrite, string filterAccept, bool multiselect,
            System.IntPtr ohPtr, ImportThenCallback thenCallback, IDBCommon.CatchCallback catchCallback);

        [DllImport("__Internal")]
        private static extern void Save2IDB_ImportToMemoryStreamsAsync(
            string filterAccept, bool multiselect, System.IntPtr ohPtr, ImportThenCallback thenCallback, IDBCommon.CatchCallback catchCallback);

        [DllImport("__Internal")]
        private static extern void Save2IDB_ReadInputtedFileAsync(
            string fileName, byte[] bytesPtr,
            System.IntPtr ohPtr, System.IntPtr readOhPtr, ReadThenCallback thenCallback, IDBCommon.CatchCallback catchCallback);

        [DllImport("__Internal")]
        private static extern void Save2IDB_DisposeImporter(System.IntPtr ohPtr);



        //////////////////////////////////////////////////////////////////////////////////
        // Constructor & Dispose
        //////////////////////////////////////////////////////////////////////////////////

        private IDBImporter() { }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                }

                var self = this;
                var selfPtr = Unsafe.As<IDBImporter, System.IntPtr>(ref self);
                Save2IDB_DisposeImporter(selfPtr);

                isDisposed = true;
            }
        }

        ~IDBImporter()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }



        //////////////////////////////////////////////////////////////////////////////////
        // Initializer
        //////////////////////////////////////////////////////////////////////////////////

        public static IDBImporter ToFile(string path, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(path)) throw new System.ArgumentException($"{nameof(path)} is null or empty.", nameof(path));
            if (Path.GetDirectoryName(path) != "/" && !Directory.Exists(path)) throw new DirectoryNotFoundException(
                $"Could not find a part of the path {Path.GetFullPath(path)}.");
            if (path.EndsWith('/')) throw new System.ArgumentException($"{path} is directory path.", nameof(path));

            var instance = new IDBImporter();
            instance.path = path;
            instance.Overwrite = overwrite;
            return instance;
        }

        public static IDBImporter InToDirectory(string path, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(path)) throw new System.ArgumentException($"{nameof(path)} is null or empty.", nameof(path));
            if (Path.GetDirectoryName(path) != "/" && !Directory.Exists(path)) throw new DirectoryNotFoundException(
                $"Could not find a part of the path {Path.GetFullPath(path)}.");
            if (!path.EndsWith('/')) { path += '/'; } // If path ends with '/' then the path is a directory path, otherwise it is a file path.

            var instance = new IDBImporter();
            instance.path = path;
            instance.Overwrite = overwrite;
            return instance;
        }

        public static IDBImporter ToMemoryStream()
        {
            var instance = new IDBImporter();
            return instance;
        }



        //////////////////////////////////////////////////////////////////////////////////
        // Import
        //////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Can only be called once.
        /// </summary>
        public IDBImporter ShowDialog()
        {
            if (isDisposed) throw new System.ObjectDisposedException(nameof(IDBImporter));
            if (isShown) throw new System.InvalidOperationException("Importer that have already been displayed cannot be displayed again.");
            isShown = true;

            if (path != null)
            {
                // To file or Into directory

                if (!path.EndsWith('/') && Multiselect) throw new System.InvalidOperationException(
                    $"Attempted to multiselect import, but 'ToFile' operation.");

#if UNITY_WEBGL && !UNITY_EDITOR
                var self = this;
                var selfPtr = Unsafe.As<IDBImporter, System.IntPtr>(ref self);
                Save2IDB_ImportToAsync(path, Overwrite, FilterAccept, Multiselect, selfPtr, ImportThen, IDBCommon.Catch);
#else
                throw new System.NotSupportedException($"{nameof(IDBImporter)} is not supported on current platform.");
#endif
            }
            else
            {
                // To MemoryStreams

#if UNITY_WEBGL && !UNITY_EDITOR
                var self = this;
                var selfPtr = Unsafe.As<IDBImporter, System.IntPtr>(ref self);
                Save2IDB_ImportToMemoryStreamsAsync(FilterAccept, Multiselect, selfPtr, ImportThen, IDBCommon.Catch);
#else
                throw new System.NotSupportedException($"{nameof(IDBImporter)} is not supported on current platform.");
#endif
            }
            return this;
        }

        [MonoPInvokeCallback(typeof(ImportThenCallback))]
        private static void ImportThen(System.IntPtr selfPtr, string statsJson)
        {
            var self = Unsafe.As<System.IntPtr, IDBImporter>(ref selfPtr);
            var stats = JsonUtility.FromJson<FileStatsArray>(statsJson);
            if (self.path != null)
            {
                // To file or Into directory

                self.Done(stats.vs.Select(x => new FileResult(self, x.destPath)).ToArray());
            }
            else
            {
                // To MemoryStreams

                self.Done(stats.vs.Select(x => new MemoryStreamResult(self, x.name, x.size)).ToArray());
            }
        }



        //////////////////////////////////////////////////////////////////////////////////
        // Result classes
        //////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// This class throws an exception if parent importer has already been Disposed.
        /// </summary>
        public abstract class ResultObj
        {
            protected IDBImporter parent;

            public string Path { get; protected set; }
            public string FileName { get; protected set; }

            internal ResultObj(IDBImporter parent) => this.parent = parent;

            public IDBOperationHandle<MemoryStream> OpenMemoryStreamAsync()
                => parent.isDisposed ? throw new System.ObjectDisposedException(nameof(IDBImporter)) : InnerOpenMemoryStreamAsync();

            protected abstract IDBOperationHandle<MemoryStream> InnerOpenMemoryStreamAsync();
        }

        // To file or Into directory
        private class FileResult : ResultObj
        {
            public FileResult(IDBImporter parent, string path)
                : base(parent)
            {
                Path = path;
                FileName = System.IO.Path.GetFileName(path);
            }

            protected override IDBOperationHandle<MemoryStream> InnerOpenMemoryStreamAsync()
            {
                var operationHandle = new IDBOperationHandle<MemoryStream>();
                operationHandle.Error($"Cannot open file as {nameof(MemoryStream)}.");
                return operationHandle;
            }
        }

        // To MemoryStreams
        private class MemoryStreamResult : ResultObj
        {
            private readonly int fileSize;

            public MemoryStreamResult(IDBImporter parent, string fileName, int fileSize)
                : base(parent)
            {
                FileName = fileName;
                this.fileSize = fileSize;
            }

            protected override IDBOperationHandle<MemoryStream> InnerOpenMemoryStreamAsync()
            {
                // Read a file inputted by user.
                var operationHandle = new ReadOperationHandle();
                operationHandle.MemoryStream = new MemoryStream();
                operationHandle.MemoryStream.SetLength(fileSize);
                var ohPtr = Unsafe.As<IDBImporter, System.IntPtr>(ref parent);
                var readOhPtr = Unsafe.As<ReadOperationHandle, System.IntPtr>(ref operationHandle);
                Save2IDB_ReadInputtedFileAsync(FileName, operationHandle.MemoryStream.GetBuffer(), ohPtr, readOhPtr, ReadThen, IDBCommon.Catch);
                return operationHandle;
            }

            [MonoPInvokeCallback(typeof(ImportThenCallback))]
            private static void ReadThen(System.IntPtr ohPtr)
            {
                var operationHandle = Unsafe.As<System.IntPtr, ReadOperationHandle>(ref ohPtr);
                operationHandle.Done(operationHandle.MemoryStream);
            }

            private class ReadOperationHandle : IDBOperationHandle<MemoryStream>
            {
                public MemoryStream MemoryStream { get; set; }
            }
        }



        //////////////////////////////////////////////////////////////////////////////////
        // For JsonUtility.FromJson
        //////////////////////////////////////////////////////////////////////////////////

        [System.Serializable]
        private class FileStatsArray
        {
            public FileStats[] vs = null;
        }

        [System.Serializable]
        private class FileStats
        {
            public string name = null;
            public string destPath = null;
            public int size = 0;
            public string type = null;
            public System.DateTime lastModifiedDate = System.DateTime.MinValue;
        }
    }
}
