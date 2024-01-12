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
            FadeCanvas.StartCanvasCoroutine(GetFilesCoroutine(path, callback));
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
            FadeCanvas.StartCanvasCoroutine(ReadCoroutine(path, callback));
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

        public static void Delete(string path)
        {
            if (path.Contains('\\')) { path = path.Replace('\\', '/'); };

#if UNITY_STANDALONE || UNITY_EDITOR
            File.Delete(path);
#elif UNITY_WEBGL
            FadeCanvas.StartCanvasCoroutine(IDBFile.DeleteAsync(path));
#else
            throw new System.NotSupportedException();
#endif
        }

        public static void Move(string sourcePath, string destPath, System.Action<bool> callback)
        {
            if (sourcePath.Contains('\\')) { sourcePath = sourcePath.Replace('\\', '/'); };
            if (destPath.Contains('\\')) { destPath = destPath.Replace('\\', '/'); };

#if UNITY_STANDALONE || UNITY_EDITOR
            if (File.Exists(destPath))
            {
                callback(false);
            }
            else
            {
                File.Move(sourcePath, destPath);
                callback(true);
            }
#elif UNITY_WEBGL
            FadeCanvas.StartCanvasCoroutine(MoveCoroutine(sourcePath, destPath, callback));
#else
            throw new System.NotSupportedException();
#endif
        }

        private static IEnumerator MoveCoroutine(string sourcePath, string destPath, System.Action<bool> callback)
        {
            var handle = IDBFile.MoveAsync(sourcePath, destPath);
            yield return handle;

            callback.Invoke(handle.Status == IDBOperationStatus.Succeeded);
        }

        public static string GetName(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public static void Export(string path)
        {
            if (path.Contains('\\')) { path = path.Replace('\\', '/'); };

#if UNITY_STANDALONE || UNITY_EDITOR
            // スタンドアロンではセーブファイルの場所を開く
            System.Diagnostics.Process.Start("explorer.exe", $"/select,{path.Replace('/', '\\')}");
#elif UNITY_WEBGL
            FadeCanvas.StartCanvasCoroutine(IDBFile.ExportAsync(path));
#else
            throw new System.NotSupportedException();
#endif
        }

        public static void Import(string path, System.Action<bool> callback)
        {
            if (path.Contains('\\')) { path = path.Replace('\\', '/'); };
            if (!path.EndsWith('/')) { path = path + '/'; }

#if UNITY_STANDALONE || UNITY_EDITOR

#elif UNITY_WEBGL
            FadeCanvas.StartCanvasCoroutine(ImportCoroutine(path, callback));
#else
            throw new System.NotSupportedException();
#endif
        }

        private static IEnumerator ImportCoroutine(string path, System.Action<bool> callback)
        {
            var handle = IDBFile.ImportAsync(path);
            yield return handle;

            callback.Invoke(handle.Status == IDBOperationStatus.Succeeded);
        }
    }
}
