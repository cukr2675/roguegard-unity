using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.IO;

namespace Save2IDB
{
    public static class IDBFileInfoExtension
    {
        /// <summary>
        /// Replaces <see cref="FileInfo.Open"/> and overrides.
        /// </summary>
        /// <param name="access">
        /// default: <paramref name="mode"/> == <see cref="FileMode.Append"/> ? <see cref="FileAccess.Write"/> : <see cref="FileAccess.ReadWrite"/>
        /// </param>
        public static void IDBOpen(this FileInfo info, FileMode mode, FileAccess access = default, FileShare share = FileShare.Read)
        {
            IDBFile.Open(info.FullName, mode, access, share);
        }
    }
}
