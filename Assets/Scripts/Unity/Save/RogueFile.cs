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
        public static void InitializeDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static FileInfo[] GetFiles(string path)
        {
            if (path.Contains('\\')) { path = path.Replace('\\', '/'); };

            var directoryInfo = new DirectoryInfo(path);
            return directoryInfo.GetFiles().OrderByDescending(x => x.LastWriteTime).ToArray();
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

        public static void Import(string path, System.Action<string> callback)
        {
            if (path.Contains('\\')) { path = path.Replace('\\', '/'); };

            var importer = IDBImporter.InToDirectory(path);
            //importer.FilterAccept = ".gard,.zip";
            importer.Completed += _ =>
            {
                try
                {
                    callback(importer.ErrorMsg);
                }
                finally
                {
                    importer.Dispose();
                }
            };
            importer.ShowDialog();
        }
    }
}
