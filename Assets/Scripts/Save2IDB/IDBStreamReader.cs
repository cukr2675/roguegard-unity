using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Runtime.InteropServices;

namespace Save2IDB
{
    unsafe public class IDBStreamReader : UnmanagedMemoryStream
    {
        [DllImport("__Internal")]
        private static extern void Save2IDB_CloseReadAllBytes(byte* pointer);

        private bool isClosed;

        internal IDBStreamReader(byte* pointer, long length)
            : base(pointer, length, length, FileAccess.Read)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (!isClosed)
            {
                Position = 0;
#if UNITY_WEBGL && !UNITY_EDITOR
                var pointer = PositionPointer;
                Save2IDB_CloseReadAllBytes(pointer);
#endif
                isClosed = true;
            }

            base.Dispose(disposing);
        }
    }
}
