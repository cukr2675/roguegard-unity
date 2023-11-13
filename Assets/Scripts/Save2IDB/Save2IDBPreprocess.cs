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
        private static readonly string directoryPath = "Assets/Save2IDBTemp";
        private static readonly string assetPath = $"{directoryPath}/{Save2IDBSettings.assetName}.asset";

        int IOrderedCallback.callbackOrder => 0;

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            var asset = Save2IDBSettings.Load();

            Directory.CreateDirectory(directoryPath);
            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.CreateAsset(asset, assetPath);

            var preloadedAssets = PlayerSettings.GetPreloadedAssets();
            preloadedAssets = preloadedAssets.Append(asset).ToArray();
            PlayerSettings.SetPreloadedAssets(preloadedAssets);
        }

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            var asset = AssetDatabase.LoadAssetAtPath<Save2IDBSettings>(assetPath);

            AssetDatabase.DeleteAsset(assetPath);
            Directory.Delete(directoryPath);
            File.Delete($"{directoryPath}.meta");

            var preloadedAssets = PlayerSettings.GetPreloadedAssets();
            preloadedAssets = preloadedAssets.Where(x => x != asset).ToArray();
            PlayerSettings.SetPreloadedAssets(preloadedAssets);
        }
    }
}
#endif
