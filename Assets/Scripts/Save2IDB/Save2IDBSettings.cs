using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Runtime.InteropServices;

namespace Save2IDB
{
    public class Save2IDBSettings : ScriptableObject
    {
        [DllImport("__Internal")]
        private static extern void Save2IDB_Initialize(string databaseName, string filesObjectStoreName);

        [SerializeField] internal bool _overrideDatabaseName = false;
        [SerializeField] internal string _databaseName = "Save2IDB";
        [SerializeField] internal string _filesObjectStoreName = "Files";

        private void OnEnable()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (!_overrideDatabaseName) { _databaseName = Application.productName; }
            Save2IDB_Initialize(_databaseName, _filesObjectStoreName);
#endif
        }

#if UNITY_EDITOR
        private string prevDatabaseName;
        private string prevFilesObjectStoreName;

        internal static readonly string assetName = typeof(Save2IDBSettings).Name;

        internal void OnValidate()
        {
            if (!_overrideDatabaseName) { _databaseName = Application.productName; }

            if (string.IsNullOrWhiteSpace(_databaseName)) { _databaseName = prevDatabaseName; }
            else { prevDatabaseName = _databaseName; }

            if (string.IsNullOrWhiteSpace(_filesObjectStoreName)) { _filesObjectStoreName = prevFilesObjectStoreName; }
            else { prevFilesObjectStoreName = _filesObjectStoreName; }
        }

        internal static Save2IDBSettings Load()
        {
            var assetPath = $"ProjectSettings/{assetName}.asset";
            var asset = (Save2IDBSettings)UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(assetPath).FirstOrDefault();
            if (asset == null)
            {
                asset = CreateInstance<Save2IDBSettings>();
                asset.name = assetName;
            }
            asset.hideFlags = HideFlags.DontSaveInEditor;
            return asset;
        }

        internal void Save()
        {
            var assetPath = $"ProjectSettings/{assetName}.asset";
            UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new[] { this }, assetPath, true);
        }
#endif
    }
}
