using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;

namespace Roguegard.Editor
{
    [CustomPropertyDrawer(typeof(ScriptField<>))]
    [CanEditMultipleObjects]
    public class ScriptFieldDrawer : PropertyDrawer
    {
        private static MonoScript _fieldAsset;

        private static MonoScript FieldAsset
        {
            get
            {
                if (_fieldAsset) return _fieldAsset;

                _fieldAsset = new MonoScript();
                return _fieldAsset;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //オブジェクトがドロップされたらパスを設定
            var script = property.FindPropertyRelative("_script");
            var _ref = property.FindPropertyRelative("_ref");

            // C# ファイル設定フォームの表示名
            var refValue = _ref.managedReferenceValue;
            FieldAsset.name = refValue?.GetType().Name;
            if (string.IsNullOrWhiteSpace(FieldAsset.name))
            {
                var typeName = _ref.managedReferenceFieldTypename;
                var lastIndex = typeName.LastIndexOf('.');
                typeName = typeName.Substring(lastIndex + 1);
                if (script.objectReferenceValue != null)
                {
                    FieldAsset.name = $"Missing ({typeName})";
                }
                else
                {
                    FieldAsset.name = $"None ({typeName})";
                }
            }

            // C# ファイル設定フォーム
            var objPosition = position;
            objPosition.height = EditorGUIUtility.singleLineHeight;
            var scriptFile = EditorGUI.ObjectField(objPosition, property.displayName, FieldAsset, typeof(MonoScript), false);
            if (scriptFile != FieldAsset)
            {
                // Inspector から obj 以外に設定されたら、その値を設定されたことにする。
                script.objectReferenceValue = scriptFile;
            }

            // インスタンスプロパティ設定フォーム
            if (HasProperties(refValue))
            {
                var instancePosition = position;
                EditorGUI.PropertyField(instancePosition, _ref, new GUIContent(), true);
            }

            // バリデーション
            var e = Event.current;
            if (e.type == EventType.Used)
            {
                Validate(property);
            }

            // C# ファイル設定フォームのフィールドをクリックしたとき、ファイルの場所をハイライトする。
            var objFieldPosition = objPosition;
            objFieldPosition.x += EditorGUIUtility.labelWidth;
            objFieldPosition.width -= EditorGUIUtility.labelWidth;
            objFieldPosition.width -= objFieldPosition.height; // フィールド右の一覧選択ボタンのぶん縮める
            if (e.type == EventType.MouseUp && objFieldPosition.Contains(e.mousePosition))
            {
                EditorGUIUtility.PingObject(script.objectReferenceValue);
            }
            else if (e.type == EventType.ContextClick && script.objectReferenceValue != null)
            {
                var scripthPath = AssetDatabase.GetAssetPath(script.objectReferenceValue);
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Edit Script"), false, () => EditorUtility.OpenWithDefaultApp(scripthPath));
                menu.ShowAsContext();
            }
        }

        public void Validate(SerializedProperty property)
        {
            var script = (MonoScript)property.FindPropertyRelative("_script").objectReferenceValue;
            var _ref = property.FindPropertyRelative("_ref");
            if (script == null)
            {
                // C# ファイルが未設定ならインスタンスも null にする。
                _ref.managedReferenceValue = null;
                //_ref.managedReferenceId = -2;
                return;
            }

            // C# ファイルからそこで定義されている型を取得する。
            var match = Regex.Match(script.text, @"namespace [a-z_A-Z]\w*(\.[a-z_A-Z]\w*)*");
            var namespaceName = match.Value.Substring("namespace ".Length);
            var scriptPath = AssetDatabase.GetAssetPath(script);
            var assemblyName = RoguegardAssetDatabase.GetAssemblyName(scriptPath);
            if (assemblyName == null) throw new System.NotSupportedException("Assembly Definition に含まれていないスクリプトは非対応です。");

            var name = $"{namespaceName}.{script.name}, {assemblyName}";
            var type = System.Type.GetType(name);
            if (type == null)
            {
                // 型の取得に失敗したまたは非対応の型だった場合、インスタンスを null にする。
                _ref.managedReferenceValue = null;
                //_ref.managedReferenceId = -2;
                Debug.LogWarning($"クラス {name} が見つかりません。");
                return;
            }
            if (!typeof(ReferableScript).IsAssignableFrom(type))
            {
                _ref.managedReferenceValue = null;
                Debug.LogWarning($"クラス {name} が見つかりましたが、 {nameof(ReferableScript)} ではありません。");
                return;
            }

            // type が既に設定されているインスタンスの型と同じ場合、何もせず終わる。
            // 新しいインスタンスで更新すると値がリセットされてしまうため、それを回避する。
            if (_ref.managedReferenceValue != null && type == _ref.managedReferenceValue.GetType()) return;

            var value = System.Activator.CreateInstance(type, true);
            try
            {
                _ref.managedReferenceValue = value;
            }
            catch
            {
                // 型が一致しないなどの理由でインスタンスの設定に失敗した場合、インスタンスを null にする。
                _ref.managedReferenceValue = null;
                //_ref.managedReferenceId = -2;
                Debug.LogWarning($"クラス {name} の設定に失敗しました。型が一致しない可能性があります。");
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var _ref = property.FindPropertyRelative("_ref");
            var height = EditorGUI.GetPropertyHeight(_ref, true);
            return height;
        }

        private static bool HasProperties(object _ref)
        {
            if (_ref == null) return false;

            var result = TypeHasProperties(_ref.GetType());
            return result;

            static bool TypeHasProperties(System.Type type)
            {
                var any = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Any(x => !x.IsNotSerialized);
                if (any) return true;
                else if (type.BaseType == typeof(ReferableScript)) return false;
                else return TypeHasProperties(type.BaseType);
            }
        }
    }
}
