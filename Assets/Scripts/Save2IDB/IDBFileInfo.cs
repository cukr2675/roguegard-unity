using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Save2IDB
{
    public class IDBFileInfo
    {
        public string Path { get; }
        public System.DateTime LastModified { get; }

        public IDBFileInfo(string path, System.DateTime lastModified)
        {
            Path = path;
            LastModified = lastModified;
        }
    }
}
