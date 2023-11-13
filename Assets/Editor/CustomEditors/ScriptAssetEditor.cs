//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using System.Text.RegularExpressions;
//using System.IO;
//using UnityEditor;
//using Roguegard.CharacterCreation;

//namespace Roguegard.Editor
//{
//    /// <summary>
//    /// <see cref="MonoScript"/> のインスペクターに <see cref="AssemblyDefaultAsset"/> の項目を表示させるエディタ拡張
//    /// </summary>
//    [CustomEditor(typeof(MonoScript))]
//    public class ScriptAssetEditor : UnityEditor.Editor
//    {
//        public override void OnInspectorGUI()
//        {
//            var script = (MonoScript)target;

//            if (GUILayout.Button("Create Default Asset"))
//            {
//                // C# ファイルからそこで定義されている型を取得する。
//                var match = Regex.Match(script.text, @"namespace [a-z_A-Z]\w*(\.[a-z_A-Z]\w*)*");
//                var namespaceName = match.Value.Substring("namespace ".Length);
//                var assemblyName = GetAssemblyName(script);
//                var name = $"{namespaceName}.{script.name}, {assemblyName}";
//                var type = System.Type.GetType(name);

//                var targetName = type.ToString();
//                var scriptPath = AssetDatabase.GetAssetPath(script);
//                var scriptDirectory = Path.GetDirectoryName(scriptPath);
//                var targetPath = $@"{scriptDirectory}\{targetName}.asset";
//                if (targetPath == scriptPath) throw new RogueException("生成によるジェネレータアセットの上書きは禁止です。");

//                var asset = AssetDatabase.LoadAssetAtPath<AssemblyDefaultAsset>(targetPath);
//                if (asset == null) { asset = CreateInstance<AssemblyDefaultAsset>(); }
//                var fieldInfo = typeof(AssemblyDefaultAsset).GetField("_ref", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
//                AssemblyReflectionDrawer.SetScript(asset, fieldInfo, script);

//                if (File.Exists(targetPath))
//                {
//                    EditorUtility.SetDirty(asset);
//                    AssetDatabase.ImportAsset(targetPath);
//                }
//                else
//                {
//                    AssetDatabase.CreateAsset(asset, targetPath);
//                }
//            }

//            static string GetAssemblyName(MonoScript script)
//            {
//                var scriptPath = AssetDatabase.GetAssetPath(script);
//                var scriptAsmdefPath = RoguegardAssetDatabase.GetNearestPath(scriptPath, "t:assemblydefinitionasset");
//                if (scriptAsmdefPath == null)
//                    throw new System.NotSupportedException("Assembly Definition に含まれていないスクリプトは非対応です。");

//                // Assembly Definition Asset のテキスト内からアセンブリ名を取り出す。
//                var asmdef = AssetDatabase.LoadAssetAtPath<UnityEditorInternal.AssemblyDefinitionAsset>(scriptAsmdefPath);
//                var head = "\"name\": \"";
//                var tail = "\"";
//                var match = Regex.Match(asmdef.text, $@"{head}[a-z_A-Z]\w*(\.[a-z_A-Z]\w*)*{tail}");
//                var assemblyName = match.Value.Substring(head.Length, match.Length - head.Length - tail.Length);
//                return assemblyName;
//            }
//        }
//    }
//}
