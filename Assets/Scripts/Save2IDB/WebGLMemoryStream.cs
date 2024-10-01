using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Runtime.InteropServices;

namespace Save2IDB
{
    internal class WebGLMemoryStream : UnmanagedMemoryStream
    {
        //[DllImport("__Internal")] unsafe private static extern void Free(byte* pointer);

        private bool isClosed;

        unsafe internal WebGLMemoryStream(byte* pointer, long length)
            : base(pointer, length, length, FileAccess.Read)
        {
        }

        ~WebGLMemoryStream()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (!isClosed)
            {
                Position = 0;
#if UNITY_WEBGL && !UNITY_EDITOR
                //unsafe { Free(PositionPointer); }
#endif
                isClosed = true;
            }

            base.Dispose(disposing);
        }
    }
}
