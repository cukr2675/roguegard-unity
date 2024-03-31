using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.IO;
using Save2IDB;

namespace RoguegardUnity
{
    public class RogueFile
    {
        public string Path { get; }
        public System.DateTime LastModified { get; }

        public delegate void Callback(string errorMsg = null);
        public delegate void Callback<T>(T result, string errorMsg = null);

        private RogueFile(string path, System.DateTime lastModified)
        {
            Path = path;
            LastModified = lastModified;
        }

        public static void GetFiles(string path, Callback<RogueFile[]> callback)
        {
            if (path.Contains('\\')) { path = path.Replace('\\', '/'); };

#if UNITY_STANDALONE || UNITY_EDITOR
            callback(Directory.GetFiles(path)
                .Select(x => new RogueFile(x.Replace('\\', '/'), File.GetLastWriteTime(x)))
                .OrderByDescending(x => x.LastModified).ToArray());
#elif UNITY_WEBGL
            FadeCanvas.StartCanvasCoroutine(GetFilesCoroutine(path, callback));
#else
            throw new System.NotSupportedException();
#endif
        }

        private static IEnumerator GetFilesCoroutine(string path, Callback<RogueFile[]> callback)
        {
            var handle = IDBFile.GetFileInfosDescDateAsync();
            yield return handle;

            callback(handle.Result?.Where(x => x.Path.StartsWith(path)).Select(x => new RogueFile(x.Path, x.LastModified)).ToArray(), handle.ErrorMsg);
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

        public static void OpenRead(string path, Callback<Stream> callback)
        {
            if (path.Contains('\\')) { path = path.Replace('\\', '/'); };

#if UNITY_STANDALONE || UNITY_EDITOR
            callback(File.OpenRead(path));
#elif UNITY_WEBGL
            FadeCanvas.StartCanvasCoroutine(ReadCoroutine(path, callback));
#else
            throw new System.NotSupportedException();
#endif
        }

        private static IEnumerator ReadCoroutine(string path, Callback<Stream> callback)
        {
            var handle = IDBFile.OpenReadAllBytesAsync(path);
            yield return handle;

            using var stream = handle.Result;
            callback.Invoke(stream, handle.ErrorMsg);
        }

        public static void Delete(string path, Callback callback)
        {
            if (path.Contains('\\')) { path = path.Replace('\\', '/'); };

#if UNITY_STANDALONE || UNITY_EDITOR
            File.Delete(path);
            callback();
#elif UNITY_WEBGL
            StartCoroutine(IDBFile.DeleteAsync(path), callback);
#else
            throw new System.NotSupportedException();
#endif
        }

        public static void Move(string sourcePath, string destPath, Callback callback, bool overwrite = false)
        {
            if (sourcePath.Contains('\\')) { sourcePath = sourcePath.Replace('\\', '/'); };
            if (destPath.Contains('\\')) { destPath = destPath.Replace('\\', '/'); };

#if UNITY_STANDALONE || UNITY_EDITOR
            if (overwrite && sourcePath != destPath && File.Exists(destPath)) { File.Delete(destPath); }
            File.Move(sourcePath, destPath);
            callback();
#elif UNITY_WEBGL
            StartCoroutine(IDBFile.MoveAsync(sourcePath, destPath, overwrite), callback);
#else
            throw new System.NotSupportedException();
#endif
        }

        public static void Exists(string path, Callback<bool> callback)
        {
            if (path.Contains('\\')) { path = path.Replace('\\', '/'); };

#if UNITY_STANDALONE || UNITY_EDITOR
            callback(File.Exists(path));
#elif UNITY_WEBGL
            StartCoroutine(IDBFile.ExistsAsync(path), callback);
#else
            throw new System.NotSupportedException();
#endif
        }

        public static string GetName(string path)
        {
            return System.IO.Path.GetFileNameWithoutExtension(path);
        }

        public static void Export(string path, Callback callback)
        {
            if (path.Contains('\\')) { path = path.Replace('\\', '/'); };

#if UNITY_STANDALONE || UNITY_EDITOR
            // スタンドアロンではセーブファイルの場所を開く
            System.Diagnostics.Process.Start("explorer.exe", $"/select,{path.Replace('/', '\\')}");
            callback();
#elif UNITY_WEBGL
            StartCoroutine(IDBFile.ExportAsync(path), callback);
#else
            throw new System.NotSupportedException();
#endif
        }

        public static void Import(string path, Callback callback)
        {
            if (path.Contains('\\')) { path = path.Replace('\\', '/'); };
            if (!path.EndsWith('/')) { path = path + '/'; }

#if UNITY_STANDALONE || UNITY_EDITOR
            callback("Not supported.");
#elif UNITY_WEBGL
            StartCoroutine(IDBFile.ImportAsync(path), callback);
#else
            throw new System.NotSupportedException();
#endif
        }

        private static void StartCoroutine(IDBOperationHandle handle, Callback callback)
        {
            FadeCanvas.StartCanvasCoroutine(Coroutine(handle, callback));

            IEnumerator Coroutine(IDBOperationHandle handle, Callback callback)
            {
                yield return handle;

                callback.Invoke(handle.ErrorMsg);
            }
        }

        private static void StartCoroutine<T>(IDBOperationHandle<T> handle, Callback<T> callback)
        {
            FadeCanvas.StartCanvasCoroutine(Coroutine(handle, callback));

            IEnumerator Coroutine(IDBOperationHandle<T> handle, Callback<T> callback)
            {
                yield return handle;

                callback.Invoke(handle.Result, handle.ErrorMsg);
            }
        }
    }
}
