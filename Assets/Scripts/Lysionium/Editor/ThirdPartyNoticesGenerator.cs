using System.IO;
using UnityEditor;
using UnityEngine;

namespace Lysionium.Editor
{
    public class ThirdPartyNoticesGenerator
    {
        [MenuItem("Tools/Generate Third Party Notices")]
        public static void GenerateNotices()
        {
            string outputPath = "ThirdPartyNotices.txt";
            string packagesPath = Path.Combine(Application.dataPath, "../Packages");

            string[] packageDirs = Directory.GetDirectories(packagesPath);
            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                writer.WriteLine("THIRD PARTY NOTICES");
                writer.WriteLine("====================\n");

                foreach (string dir in packageDirs)
                {
                    string licensePath = Path.Combine(dir, "LICENSE.txt");
                    if (File.Exists(licensePath))
                    {
                        string packageName = Path.GetFileName(dir);
                        writer.WriteLine($"Package: {packageName}\n");

                        string licenseText = File.ReadAllText(licensePath);
                        writer.WriteLine(licenseText);
                        writer.WriteLine("\n------------------------\n");
                    }
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"ThirdPartyNotices.txt generated at: {Path.GetFullPath(outputPath)}");
        }
    }
}
