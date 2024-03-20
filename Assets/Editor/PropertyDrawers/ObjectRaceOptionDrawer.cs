using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Reflection;
using UnityEditor;
using Roguegard;
using Roguegard.CharacterCreation;

namespace RoguegardUnity
{
    [CustomPropertyDrawer(typeof(ObjectRaceOption), true)]
    public class ObjectRaceOptionDrawer : PropertyDrawer
    {
        private static int selected;
        private static readonly string[] items = new[] { "Active Properties", "All Properties" };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var width = 300f;
            var y = position.y + EditorGUIUtility.standardVerticalSpacing;

            var headerPosition = new Rect(position.x, y, width, EditorGUIUtility.singleLineHeight * 1.5f);
            GUI.Label(headerPosition, "Race Option", EditorStyles.boldLabel);

            var toolbarPosition = new Rect(position.x + EditorGUIUtility.labelWidth / 2f, y, width, EditorGUIUtility.singleLineHeight);
            selected = GUI.Toolbar(toolbarPosition, selected, items);

            position.y += headerPosition.height;

            if (selected == 1)
            {
                EditorGUI.PropertyField(position, property, true);
            }
            else
            {
                RaceGUI(position, property);
            }
        }

        private static void RaceGUI(Rect position, SerializedProperty property)
        {
            var y = position.y;
            var path = property.propertyPath;
            var parentType = property.serializedObject.targetObject.GetType();
            var raceField = parentType.GetField(path, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var raceType = raceField.FieldType;
            while (property.NextVisible(true) && property.propertyPath.Contains(path)) // 次に移動できない or 親プロパティに出た 場合は終了
            {
                // 子プロパティは表示しない。
                if (property.propertyPath.LastIndexOf('.') != path.Length) continue;

                // デフォルト値を省略する。ただし Header 属性が設定されているものは除く。
                if (IsDefault(property) && !AnyHeader(property, raceType)) continue;

                var height = EditorGUI.GetPropertyHeight(property, true);
                var memberPosition = new Rect(position.x, y, position.width, height);
                y += height + EditorGUIUtility.standardVerticalSpacing;

                EditorGUI.PropertyField(memberPosition, property, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUIUtility.singleLineHeight * 1.5f;
            if (selected == 1)
            {
                height += EditorGUI.GetPropertyHeight(property);
            }
            else
            {
                var path = property.propertyPath;
                var parentType = property.serializedObject.targetObject.GetType();
                var raceField = parentType.GetField(path, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var raceType = raceField.FieldType;
                while (property.NextVisible(true) && property.propertyPath.Contains(path)) // 次に移動できない or 親プロパティに出た 場合は終了
                {
                    // 子プロパティは表示しない。
                    if (property.propertyPath.LastIndexOf('.') != path.Length) continue;

                    // デフォルト値を省略する。ただし Header 属性が設定されているものは除く。
                    if (IsDefault(property) && !AnyHeader(property, raceType)) continue;

                    height += EditorGUI.GetPropertyHeight(property, true);
                    height += EditorGUIUtility.standardVerticalSpacing;
                }
            }
            return height;
        }

        private static bool IsDefault(SerializedProperty property)
        {
            if (property.propertyType == SerializedPropertyType.String && string.IsNullOrWhiteSpace(property.stringValue)) return true;

            if (property.propertyType == SerializedPropertyType.Integer && property.intValue == 0) return true;
            if (property.propertyType == SerializedPropertyType.Float && property.floatValue == 0f) return true;

            if (property.isArray && property.arraySize == 0) return true;

            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null) return true;

            if (property.type == typeof(ScriptField<>).Name)
            {
                var script = property.FindPropertyRelative("_script");
                if (script != null && script.objectReferenceValue == null) return true;
            }

            return false;
        }

        private static bool AnyHeader(SerializedProperty property, System.Type raceType)
        {
            var searchType = raceType;
            FieldInfo fieldInfo = null;
            while (fieldInfo == null && searchType != null)
            {
                fieldInfo ??= searchType.GetField(property.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                searchType = searchType.BaseType;
            }
            return fieldInfo.CustomAttributes.Any(x => x.AttributeType == typeof(HeaderAttribute));
        }
    }
}
