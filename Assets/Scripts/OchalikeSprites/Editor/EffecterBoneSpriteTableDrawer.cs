//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using System.Linq;
//using System.Reflection;
//using UnityEditor;

//namespace OchalikeSprites.Editor
//{
//    [CustomPropertyDrawer(typeof(EffecterBoneSpriteTableData.Item), true)]
//    public class EffecterBoneSpriteTableDrawer : PropertyDrawer
//    {
//        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//        {
//            var y = position.y + EditorGUIUtility.standardVerticalSpacing;
//            var path = property.propertyPath;
//            var parentType = property.serializedObject.targetObject.GetType();
//            var raceField = parentType.GetField(path, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
//            var raceType = raceField?.FieldType;
//            while (property.NextVisible(true) && property.propertyPath.Contains(path)) // 次に移動できない or 親プロパティに出た 場合は終了
//            {
//                // 子プロパティは表示しない。
//                if (property.propertyPath.LastIndexOf('.') != path.Length) continue;

//                // デフォルト値を省略する。ただし Header 属性が設定されているものは除く。
//                if (IsDefault(property) && !AnyHeader(property, raceType)) continue;

//                var height = EditorGUI.GetPropertyHeight(property, true);
//                var memberPosition = new Rect(position.x, y, position.width, height);
//                y += height + EditorGUIUtility.standardVerticalSpacing;

//                EditorGUI.PropertyField(memberPosition, property, true);
//            }
//        }

//        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//        {
//            var height = EditorGUIUtility.singleLineHeight * 1.5f;
//            {
//                var path = property.propertyPath;
//                var parentType = property.serializedObject.targetObject.GetType();
//                var raceField = parentType.GetField(path, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
//                var raceType = raceField?.FieldType;
//                while (property.NextVisible(true) && property.propertyPath.Contains(path)) // 次に移動できない or 親プロパティに出た 場合は終了
//                {
//                    // 子プロパティは表示しない。
//                    if (property.propertyPath.LastIndexOf('.') != path.Length) continue;

//                    // デフォルト値を省略する。ただし Header 属性が設定されているものは除く。
//                    if (IsDefault(property) && !AnyHeader(property, raceType)) continue;

//                    height += EditorGUI.GetPropertyHeight(property, true);
//                    height += EditorGUIUtility.standardVerticalSpacing;
//                }
//            }
//            return height;
//        }

//        private static bool IsDefault(SerializedProperty property)
//        {
//            if (property.propertyType == SerializedPropertyType.String && string.IsNullOrWhiteSpace(property.stringValue)) return true;

//            if (property.propertyType == SerializedPropertyType.Integer && property.intValue == 0) return true;
//            if (property.propertyType == SerializedPropertyType.Float && property.floatValue == 0f) return true;

//            if (property.propertyType == SerializedPropertyType.Boolean && property.boolValue == false) return true;
//            if (property.propertyType == SerializedPropertyType.Color && property.colorValue == Color.white) return true;

//            if (property.isArray && property.arraySize == 0) return true;

//            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null) return true;

//            return false;
//        }

//        private static bool AnyHeader(SerializedProperty property, System.Type raceType)
//        {
//            var searchType = raceType;
//            FieldInfo fieldInfo = null;
//            while (fieldInfo == null && searchType != null)
//            {
//                fieldInfo ??= searchType.GetField(property.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
//                searchType = searchType.BaseType;
//            }
//            return fieldInfo?.CustomAttributes.Any(x => x.AttributeType == typeof(HeaderAttribute)) ?? false;
//        }
//    }
//}
