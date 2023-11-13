using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Roguegard.Editor
{
    [CreateAssetMenu(menuName = "RoguegardData/Editor/ConstantGenerator")]
    public class AssetConstantGenerator : ScriptableGenerator
    {
        [SerializeField] private string _namespaceName = null;
        [SerializeField] private DefaultAsset _targetFolder = null;
        [SerializeField] private string[] _assetTypeNames = null;
        [SerializeField] private string _propertyTypeName = null;

        private string FindFilter => string.Join(' ', _assetTypeNames.Select(x => $"t: {x}"));

        private void OnValidate()
        {
            var targetFolderPath = AssetDatabase.GetAssetPath(_targetFolder);
            if (!Directory.Exists(targetFolderPath))
            {
                // 設定された値がフォルダでない場合、無効としてリセットする。
                _targetFolder = null;
            }
        }

        protected sealed override void Generate()
        {
            var thisPath = AssetDatabase.GetAssetPath(this);
            var thisDirectory = Path.GetDirectoryName(thisPath);
            var targetPath = $@"{thisDirectory}\{name}.cs";
            if (targetPath == thisPath) throw new RogueException();

            var targetFolderPath = AssetDatabase.GetAssetPath(_targetFolder);
            var assetGUIDs = AssetDatabase.FindAssets(FindFilter, new[] { targetFolderPath });
            var assetItems = assetGUIDs.Select(x => new Item(x)).ToArray();

            var writer = new StreamWriter(targetPath);

            writer.Write(
$@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace {_namespaceName}
{{
    public class {name} : {typeof(ScriptableLoader).Name}
    {{
        private static {name} instance;");

            // SerializeField + 静的プロパティ
            foreach (var item in assetItems)
            {
                writer.Write($@"

        [SerializeField] private {item.TypeName} {item.FieldName};
        public static {_propertyTypeName} {item.Name} => instance.{item.FieldName};");
            }

            // 非同期初期化
            writer.Write($@"

        public override IEnumerator LoadAsync()
        {{
            instance = this;
            yield break;
        }}");

            // テスト用の同期初期化
            writer.Write($@"

        public override void TestLoad()
        {{
{"#if UNITY_EDITOR"}
            instance = this;
{"#else"}
            throw new RogueException(""This method is Editor Only."");
{"#endif"}
        }}
    }}
}}
");

            writer.Close();
            AssetDatabase.ImportAsset(targetPath);
            AssetDatabase.Refresh();
        }

        private class Item
        {
            public string GUID { get; }
            public Object Asset { get; }
            public string Name { get; }
            public string FieldName { get; }
            public string TypeName { get; }

            public Item(string guid)
            {
                GUID = guid;
                var path = AssetDatabase.GUIDToAssetPath(guid);
                Asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                Name = Path.GetFileNameWithoutExtension(path);
                FieldName = "_" + Regex.Replace(Name, @"^[A-Z]+", x => x.Value.ToLowerInvariant());
                TypeName = Asset.GetType().Name;
            }
        }

        [CustomEditor(typeof(AssetConstantGenerator))]
        [CanEditMultipleObjects]
        private class Editor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                var anyRequireUpdate = GetRequireUpdate(targets);
                if (!anyRequireUpdate) { EditorGUI.BeginDisabledGroup(true); }

                if (GUILayout.Button("Generate Script"))
                {
                    foreach (var target in targets)
                    {
                        ((AssetConstantGenerator)target).Generate();
                    }
                }

                if (GUILayout.Button("Generate Scriptable Object"))
                {
                    foreach (var item in targets)
                    {
                        var target = (AssetConstantGenerator)item;
                        var assetName = target.name;
                        var targetPath = AssetDatabase.GetAssetPath(target);
                        var targetDirectory = Path.GetDirectoryName(targetPath);
                        var assetPath = $@"{targetDirectory}\{assetName}.init.asset";
                        if (assetPath == targetPath) throw new RogueException("生成によるジェネレータアセットの上書きは禁止です。");

                        var assemblyName = RoguegardAssetDatabase.GetAssemblyName(targetPath);
                        var assetType = System.Type.GetType($"{target._namespaceName}.{ target.name}, {assemblyName}");
                        var asset = AssetDatabase.LoadAssetAtPath(assetPath, assetType);
                        if (asset == null) { asset = CreateInstance(assetType); }

                        // リフレクションで値を自動設定する
                        var targetFolderPath = AssetDatabase.GetAssetPath(target._targetFolder);
                        var assetGUIDs = AssetDatabase.FindAssets(target.FindFilter, new[] { targetFolderPath });
                        var assetItems = assetGUIDs.Select(x => new Item(x)).ToArray();
                        foreach (var assetItem in assetItems)
                        {
                            var fieldInfo = assetType.GetField(
                                assetItem.FieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            fieldInfo.SetValue(asset, assetItem.Asset);
                        }

                        if (File.Exists(assetPath))
                        {
                            EditorUtility.SetDirty(asset);
                            AssetDatabase.ImportAsset(assetPath);
                        }
                        else
                        {
                            AssetDatabase.CreateAsset(asset, assetPath);
                        }
                    }
                }

                if (!anyRequireUpdate) { EditorGUI.EndDisabledGroup(); }
            }

            private static bool GetRequireUpdate(Object[] targets)
            {
                foreach (var item in targets)
                {
                    var target = (AssetConstantGenerator)item;
                    var assetName = target.name;
                    var targetPath = AssetDatabase.GetAssetPath(target);
                    var targetDirectory = Path.GetDirectoryName(targetPath);
                    var assetPath = $@"{targetDirectory}\{assetName}.init.asset";
                    if (assetPath == targetPath) throw new RogueException("生成によるジェネレータアセットの上書きは禁止です。");

                    var assemblyName = RoguegardAssetDatabase.GetAssemblyName(targetPath);
                    var assetType = System.Type.GetType($"{target._namespaceName}.{ target.name}, {assemblyName}");
                    var asset = AssetDatabase.LoadAssetAtPath(assetPath, assetType);
                    if (asset == null) return true;

                    var targetFolderPath = AssetDatabase.GetAssetPath(target._targetFolder);
                    var assetGUIDs = AssetDatabase.FindAssets(target.FindFilter, new[] { targetFolderPath });
                    var fieldInfos = assetType.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (fieldInfos.Length != assetGUIDs.Length) return true;

                    var assetItems = assetGUIDs.Select(x => new Item(x)).ToArray();
                    foreach (var assetItem in assetItems)
                    {
                        var fieldInfo = assetType.GetField(
                            assetItem.FieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (fieldInfo == null) return true;

                        var value = fieldInfo.GetValue(asset);
                        if ((value as Object) != assetItem.Asset) return true;
                    }
                }
                return false;
            }
        }
    }
}
