using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Save2IDB
{
    public class Save2IDBSettings : ScriptableObject
    {
        [DllImport("__Internal")]
        private static extern void Save2IDB_Initialize(string databaseName, string filesObjectStoreName);

        [SerializeField] internal string _databaseName = defaultDatabaseName;
        [SerializeField] internal string _filesObjectStoreName = defaultFilesObjectStoreName;

        private const string defaultDatabaseName = "Save2IDB/%md5_hash_of_data_path%";
        private const string defaultFilesObjectStoreName = "Files";

        private void OnEnable()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            var hash = GetHash();
            var databaseName = GetName(_databaseName, hash);
            var filesObjectStoreName = GetName(_filesObjectStoreName, hash);
            Save2IDB_Initialize(databaseName, filesObjectStoreName);
#endif
        }

        private static string GetHash()
        {
            var path = Application.dataPath; // パーセントエンコーディング済み
            var pathBytes = Encoding.ASCII.GetBytes(path); // ハッシュ生成のためbyte配列に変換

            using var md5 = new MD5CryptoServiceProvider();
            var hashBytes = md5.ComputeHash(pathBytes); // ハッシュを格納したbyte配列を生成
            var hash = string.Join("", hashBytes.Select(x => x.ToString("x2"))); // byte配列を16進数表記で文字列に変換
            return hash;
        }

        private static string GetName(string format, string hash)
        {
            // %companyname% を置き換える。 %% でエスケープ可能（%%companyname% や %companyname%% は置き換えない）
            format = Regex.Replace(format, @"(?<!%)%companyname%(?!%)", Application.companyName);
            format = Regex.Replace(format, @"(?<!%)%productname%(?!%)", Application.productName);
            format = Regex.Replace(format, @"(?<!%)%md5_hash_of_data_path%(?!%)", hash);
            format = format.Replace("%%", "%"); // エスケープされた % を戻す
            return format;
        }

#if UNITY_EDITOR
        private string prevDatabaseName;
        private string prevFilesObjectStoreName;

        internal static readonly string assetName = typeof(Save2IDBSettings).Name;

        internal void OnValidate()
        {
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
