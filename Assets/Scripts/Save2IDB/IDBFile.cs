using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

namespace Save2IDB
{
    public static class IDBFile
    {
        /// <summary>
        /// Replaces <see cref="File.Open"/> and overrides.
        /// </summary>
        /// <param name="access">
        /// default: <paramref name="mode"/> == <see cref="FileMode.Append"/> ? <see cref="FileAccess.Write"/> : <see cref="FileAccess.ReadWrite"/>
        /// </param>
        public static FileStream Open(string path, FileMode mode, FileAccess access = default, FileShare share = FileShare.Read)
        {
            access = GetAccess(mode, access);

#if UNITY_WEBGL
            return new IDBFileStream(path, mode, access, share);
#else
            return new FileStream(path, mode, access, share);
#endif
        }

        internal static FileAccess GetAccess(FileMode mode, FileAccess access)
        {
            if (access != default) return access;
            if (mode == FileMode.Append) return FileAccess.Write;
            return FileAccess.ReadWrite;
        }
    }
}
