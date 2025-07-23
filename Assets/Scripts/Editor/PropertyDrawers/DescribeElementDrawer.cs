using UnityEditor;
using UnityEngine;

namespace Roguegard.Editor
{
    [CustomPropertyDrawer(typeof(DescribeElementAttribute))]
    public class DescribeElementDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = (DescribeElementAttribute)this.attribute;
            var objPath = $"{property.propertyPath}.{attribute.DescribeVariableName}";
            var objProperty = property.serializedObject.FindProperty(objPath);
            var obj = (IRogueDescribable)objProperty.objectReferenceValue;
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
