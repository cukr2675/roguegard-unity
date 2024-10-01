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

        private RogueFile(string path, System.DateTime lastModified)
        {
            Path = path;
            LastModified = lastModified;
        }

        public static void InitializeDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static RogueFile[] GetFiles(string path)
        {
            if (path.Contains('\\')) { path = path.Replace('\\', '/'); };

            return Directory.GetFiles(path)
                .Select(x => new RogueFile(x.Replace('\\', '/'), File.GetLastWriteTime(x)))
                .OrderByDescending(x => x.LastModified).ToArray();
        }

        public static FileStream Create(string path)
        {
            if (path.Contains('\\')) { path = path.Replace('\\', '/'); };

            return IDBFile.Open(path, FileMode.Create);
        }

        public static FileStream OpenRead(string path)
        {
            if (path.Contains('\\')) { path = path.Replace('\\', '/'); };

            return IDBFile.Open(path, FileMode.Open, FileAccess.Read);
        }

        public static void Delete(string path)
        {
            if (path.Contains('\\')) { path = path.Replace('\\', '/'); };

            File.Delete(path);
        }

        public static void Move(string sourcePath, string destPath)
        {
            if (sourcePath.Contains('\\')) { sourcePath = sourcePath.Replace('\\', '/'); };
            if (destPath.Contains('\\')) { destPath = destPath.Replace('\\', '/'); };

            File.Move(sourcePath, destPath);
        }

        public static bool Exists(string path)
        {
            if (path.Contains('\\')) { path = path.Replace('\\', '/'); };

            return File.Exists(path);
        }

        public static string GetName(string path)
        {
            return System.IO.Path.GetFileNameWithoutExtension(path);
        }

        public static void Export(string path)
        {
            if (path.Contains('\\')) { path = path.Replace('\\', '/'); };

            var exporter = IDBExporter.FromFile(path);
            exporter.Export();
        }

        public static void Import(string path, Callback callback)
        {
            if (path.Contains('\\')) { path = path.Replace('\\', '/'); };

            var importer = IDBImporter.InToDirectory(path);
            importer.FilterAccept = ".gard,.zip";
            importer.Completed += _ => callback(importer.ErrorMsg);
            importer.ShowDialog();
        }
    }
}
