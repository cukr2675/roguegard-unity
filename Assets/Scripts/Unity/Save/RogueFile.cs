using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.IO;
using Save2IDB;

namespace RoguegardUnity
{
    public static class RogueFile
    {
        public static void GetFiles(string path, System.Action<string[]> callback)
        {
            if (path.Contains('\\')) { path = path.Replace('\\', '/'); };

#if UNITY_STANDALONE || UNITY_EDITOR
            callback(Directory.GetFiles(path));
#elif UNITY_WEBGL
            FadeCanvas.StartConvasCoroutine(GetFilesCoroutine(path, callback));
#else
            throw new System.NotSupportedException();
#endif
        }

        private static IEnumerator GetFilesCoroutine(string path, System.Action<string[]> callback)
        {
            var handle = IDBFile.GetFileInfosDescDateAsync();
            yield return handle;

            callback(handle.Result.Select(x => x.Path).Where(x => x.StartsWith(path)).ToArray());
        }

        public static RogueSaveStream Create(string path)
        {
            if (path.Contains('\\')) { path = path.Replace('\\', '/'); };

#if UNITY_STANDALONE || UNITY_EDITOR
            return new FileRogueSaveStream(path);
#elif UNITY_WEBGL
            return new IDBRogueSaveStream(path);
#else
            throw new System.NotSupportedException();
#endif
        }

        public static void OpenRead(string path, System.Action<Stream> callback)
        {
            if (path.Contains('\\')) { path = path.Replace('\\', '/'); };

#if UNITY_STANDALONE || UNITY_EDITOR
            callback(File.OpenRead(path));
#elif UNITY_WEBGL
            FadeCanvas.StartConvasCoroutine(ReadCoroutine(path, callback));
#else
            throw new System.NotSupportedException();
#endif
        }

        private static IEnumerator ReadCoroutine(string path, System.Action<Stream> callback)
        {
            var handle = IDBFile.OpenReadAsync(path);
            yield return handle;

            using var stream = handle.Result;
            callback.Invoke(stream);
        }
    }
}
