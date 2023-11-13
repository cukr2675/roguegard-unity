using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using Roguegard;
using Roguegard.CharacterCreation;

namespace RoguegardUnity
{
    [CustomPropertyDrawer(typeof(ObjectRaceOption), true)]
    public class ObjectRaceOptionDrawer : PropertyDrawer
    {
        private static bool toggle;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var togglePosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            toggle = EditorGUI.Toggle(togglePosition, toggle);
            position.y += togglePosition.height;

            if (toggle)
            {
                EditorGUI.PropertyField(position, property, true);
            }
            else
            {
                RaceGUI(position, property);
            }
        }

        private void RaceGUI(Rect position, SerializedProperty property)
        {
            var y = position.y;
            var path = property.propertyPath;
            while (property.NextVisible(true) && property.propertyPath.Contains(path)) // 次に移動できない or 親プロパティに出た 場合は終了
            {
                // 子プロパティは表示しない。
                if (property.propertyPath.LastIndexOf('.') != path.Length) continue;

                // デフォルト値を省略する。
                if (IsDefault(property)) continue;

                var height = EditorGUI.GetPropertyHeight(property, true);
                var memberPosition = new Rect(position.x, y, position.width, height);
                y += height + EditorGUIUtility.standardVerticalSpacing;

                EditorGUI.PropertyField(memberPosition, property, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUIUtility.singleLineHeight;
            if (toggle)
            {
                height += EditorGUI.GetPropertyHeight(property);
            }
            else
            {
                var path = property.propertyPath;
                while (property.NextVisible(true) && property.propertyPath.Contains(path)) // 次に移動できない or 親プロパティに出た 場合は終了
                {
                    // 子プロパティは表示しない。
                    if (property.propertyPath.LastIndexOf('.') != path.Length) continue;

                    // デフォルト値を省略する。
                    if (IsDefault(property)) continue;

                    height += EditorGUI.GetPropertyHeight(property, true);
                    height += EditorGUIUtility.standardVerticalSpacing;
                }
            }
            return height;
        }

        private static bool IsDefault(SerializedProperty property)
        {
            if (property.name.ToLower().StartsWith("_equipped"))
            {
            }

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
    }
}
