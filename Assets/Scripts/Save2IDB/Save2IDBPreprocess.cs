#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Save2IDB
{
    internal class Save2IDBPreprocess : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private static readonly string tempDirectoryPath = "Assets/Save2IDBTemp";
        private static readonly string tempAssetPath = $"{tempDirectoryPath}/{typeof(Save2IDBSettings).Name}.asset";

        int IOrderedCallback.callbackOrder => 0;

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            var asset = Save2IDBSettings.Load();

            Directory.CreateDirectory(tempDirectoryPath);
            AssetDatabase.DeleteAsset(tempAssetPath);
            AssetDatabase.CreateAsset(asset, tempAssetPath);

            var preloadedAssets = PlayerSettings.GetPreloadedAssets();
            preloadedAssets = preloadedAssets.Append(asset).ToArray();
            PlayerSettings.SetPreloadedAssets(preloadedAssets);
        }

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            var asset = AssetDatabase.LoadAssetAtPath<Save2IDBSettings>(tempAssetPath);

            AssetDatabase.DeleteAsset(tempAssetPath);
            Directory.Delete(tempDirectoryPath);
            File.Delete($"{tempDirectoryPath}.meta");

            var preloadedAssets = PlayerSettings.GetPreloadedAssets();
            preloadedAssets = preloadedAssets.Where(x => x != asset).ToArray();
            PlayerSettings.SetPreloadedAssets(preloadedAssets);
        }
    }
}
#endif
