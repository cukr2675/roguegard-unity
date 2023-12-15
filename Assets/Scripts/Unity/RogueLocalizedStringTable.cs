using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using Roguegard;

namespace RoguegardUnity
{
    [CreateAssetMenu(menuName = "RoguegardData/Settings/RogueLocalizedStringTable")]
    public class RogueLocalizedStringTable : ScriptableLoader
    {
        [SerializeField] private string[] _tableCollectionNames = null;
        private static StringTable[] tables;

        public override float ProgressBarWeight => 4f;

        public static bool TryGetEntry(string key, out StringTableEntry entry)
        {
            foreach (var table in tables)
            {
                entry = table.GetEntry(key);
                if (entry != null) return true;
            }

            Debug.LogWarning($"{key} のエントリが見つかりません。");
            entry = null;
            return false;
        }

        public override IEnumerator LoadAsync()
        {
            tables = new StringTable[_tableCollectionNames.Length];
            for (int i = 0; i < _tableCollectionNames.Length; i++)
            {
                var tableCollectionName = _tableCollectionNames[i];
                var loadTable = new LocalizedStringTable { TableReference = tableCollectionName }.GetTableAsync();
                yield return loadTable;

                if (loadTable.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError($"ローカライズ {tableCollectionName} の読み込みに失敗しました。");
                }

                tables[i] = loadTable.Result;
            }
            yield break;
        }

        public override void TestLoad()
        {
#if UNITY_EDITOR
            tables = new StringTable[_tableCollectionNames.Length];
            for (int i = 0; i < _tableCollectionNames.Length; i++)
            {
                var tableCollectionName = _tableCollectionNames[i];
                var table = new LocalizedStringTable { TableReference = tableCollectionName }.GetTable();
                tables[i] = table;
            }
#endif
        }

#if UNITY_EDITOR
        [ContextMenu("Update")]
        private void Update()
        {
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(StringTable)}");
            var tableCollectionNames = new HashSet<string>();
            foreach (var guid in guids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<StringTable>(path);
                tableCollectionNames.Add(asset.TableCollectionName);
            }
            _tableCollectionNames = tableCollectionNames.ToArray();
        }
#endif
    }
}
