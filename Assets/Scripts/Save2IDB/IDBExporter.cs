using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Runtime.InteropServices;

namespace Save2IDB
{
    public class IDBExporter
    {
        private string path;
        private byte[] bytes;
        private int offset;
        private int count;

        public string FileNameAs { get; set; }
        public string ContentType { get; set; }

        [DllImport("__Internal")]
        private static extern void Save2IDB_ExportFrom(string path, string filename, string contentType);

        [DllImport("__Internal")]
        unsafe private static extern void Save2IDB_ExportAllBytes(byte* bytesPtr, int bytesLen, string filename, string contentType);

        private IDBExporter() { }

        public static IDBExporter FromFile(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new System.ArgumentException($"{nameof(path)} is null or empty.", nameof(path));

            var instance = new IDBExporter();
            instance.path = path;
            instance.FileNameAs = Path.GetFileName(path);
            return instance;
        }

        public static IDBExporter FromBytes(byte[] bytes, int offset, int count)
        {
            if (bytes == null) throw new System.ArgumentNullException(nameof(bytes));
            if (offset < 0 || bytes.Length < offset) throw new System.ArgumentOutOfRangeException(nameof(offset));
            if (count < 0 || bytes.Length - offset < count) throw new System.ArgumentOutOfRangeException(nameof(count));

            var instance = new IDBExporter();
            instance.bytes = bytes;
            instance.offset = offset;
            instance.count = count;
            return instance;
        }

        public void Export()
        {
            if (path != null)
            {
                // From file

#if UNITY_WEBGL && !UNITY_EDITOR
                Save2IDB_ExportFrom(path, FileNameAs, ContentType);
#else
                throw new System.NotSupportedException($"{nameof(IDBExporter)} is not supported on current platform.");
#endif
            }
            else
            {
                // From bytes

#if UNITY_WEBGL && !UNITY_EDITOR
                unsafe
                {
                    fixed (byte* bytesPtr = &bytes[offset])
                    {
                        Save2IDB_ExportAllBytes(bytesPtr, count, FileNameAs, ContentType);
                    }
                }
#else
                throw new System.NotSupportedException($"{nameof(IDBExporter)} is not supported on current platform.");
#endif
            }
        }
    }
}
