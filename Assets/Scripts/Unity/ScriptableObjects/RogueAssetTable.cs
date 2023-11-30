using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Reflection;

namespace RoguegardUnity
{
    [CreateAssetMenu(menuName = "RoguegardData/Settings/RogueAssetTable")]
    public class RogueAssetTable : ScriptableObject, IReadOnlyDictionary<string, object>
    {
#if UNITY_EDITOR
        [SerializeField] private string _namespace = null;
        [SerializeField] private string _filter = "t:scriptableobject";
        [SerializeField] private UnityEditor.DefaultAsset[] _targetFolders = null;
#endif

        [SerializeField] private List<Item> _items = new List<Item>();

        private Dictionary<string, object> _table;
        private Dictionary<string, object> Table => _table ??= new Dictionary<string, object>(_items.Select(x => x.ToPair()));

        [ContextMenu("ForceUpdate")]
        private void ForceUpdate()
        {
            Update(true);
        }

        public void Update()
        {
            Update(false);
        }

        private void Update(bool force)
        {
#if UNITY_EDITOR
            var newItems = new List<Item>();
            var needItemKeys = new List<string>(_items.Select(x => x.Key));
            foreach (var folder in _targetFolders)
            {
                var folderPath = UnityEditor.AssetDatabase.GetAssetPath(folder);
                var assetGUIDs = UnityEditor.AssetDatabase.FindAssets(_filter, new[] { folderPath });
                foreach (var assetGUID in assetGUIDs)
                {
                    var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(assetGUID);
                    var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                    var assetType = asset.GetType();

                    // Referable である ScriptableObject だけ登録する
                    if (!assetType.IsDefined(typeof(ObjectFormer.ReferableAttribute))) continue;

                    var name = $"{_namespace}.{asset.name}";
                    var item = new Item(name, asset);
                    newItems.Add(item);
                    needItemKeys.Remove(name);
                }
            }

            if (!force && needItemKeys.Count >= 1)
            {
                foreach (var needItemKey in needItemKeys)
                {
                    Debug.LogError($"{needItemKey} のアセットが見つからないため {name} を更新できません。");
                }
            }
            else
            {
                _items = newItems;
                //var thisPath = AssetDatabase.GetAssetPath(this);
                //EditorUtility.SetDirty(this);
                //AssetDatabase.ImportAsset(thisPath);
            }
#endif
        }

        object IReadOnlyDictionary<string, object>.this[string key] => Table[key];
        IEnumerable<string> IReadOnlyDictionary<string, object>.Keys => Table.Keys;
        IEnumerable<object> IReadOnlyDictionary<string, object>.Values => Table.Values;
        int IReadOnlyCollection<KeyValuePair<string, object>>.Count => Table.Count;
        bool IReadOnlyDictionary<string, object>.ContainsKey(string key) => Table.ContainsKey(key);
        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => Table.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Table.GetEnumerator();
        bool IReadOnlyDictionary<string, object>.TryGetValue(string key, out object value) => Table.TryGetValue(key, out value);

        [System.Serializable]
        private class Item
        {
            [SerializeField] private string _key;
            public string Key => _key;

            [SerializeField] private Object _asset;
            public Object Asset => _asset;

            public Item() { }

            public Item(string key, Object asset)
            {
                _key = key;
                _asset = asset;
            }

            public KeyValuePair<string, object> ToPair() => new KeyValuePair<string, object>(Key, Asset);
        }
    }
}
