using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace Roguegard.Editor
{
    [CustomPropertyDrawer(typeof(ElementDescriptionAttribute))]
    public class ElementDescriptionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = (ElementDescriptionAttribute)this.attribute;
            var objPath = $"{property.propertyPath}.{attribute.DescriptionVariableName}";
            var objProperty = property.serializedObject.FindProperty(objPath);
            var obj = (IRogueDescription)objProperty.objectReferenceValue;
            if (obj == null)
            {
                EditorGUI.PropertyField(position, property, true);
                return;
            }

            var preview = AssetPreview.GetAssetPreview(obj.Icon);
            EditorGUI.PropertyField(position, property, new GUIContent(obj.Name ?? label.text, preview), true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
