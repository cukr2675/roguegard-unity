using UnityEditor;
using UnityEngine;

namespace OchalikeSprites.Editor
{
    [CustomPropertyDrawer(typeof(VisibleByAttribute))]
    public class VisibleByDrawer : PropertyDrawer
    {
        private const float fixedWidth = 64f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = (VisibleByAttribute)this.attribute;
            var path = property.propertyPath;
            path = path[..(path.LastIndexOf('.') + 1)];
            var disabled = false;
            if (attribute.BooleanName != null)
            {
                var booleanPath = path + attribute.BooleanName;
                var boolean = property.serializedObject.FindProperty(booleanPath);
                disabled = !boolean.boolValue;
            }

            if (!disabled)
            {
                if (attribute.FixedName != null)
                {
                    var fixedPath = path + attribute.FixedName;
                    var isFixed = property.serializedObject.FindProperty(fixedPath);
                    EditorGUI.PropertyField(new Rect(position.x, position.y, position.width - fixedWidth, position.height), property, true);
                    var beforeLabelWidth = EditorGUIUtility.labelWidth;
                    var beforeIndentLevel = EditorGUI.indentLevel;
                    EditorGUIUtility.labelWidth = 48f;
                    EditorGUI.indentLevel = 1;
                    Debug.Log(isFixed.displayName);
                    EditorGUI.PropertyField(new Rect(position.xMax - fixedWidth, position.y, fixedWidth, position.height), isFixed, new GUIContent("Fixed"), true);
                    EditorGUIUtility.labelWidth = beforeLabelWidth;
                    EditorGUI.indentLevel = beforeIndentLevel;
                }
                else
                {
                    EditorGUI.PropertyField(position, property, true);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attribute = (VisibleByAttribute)this.attribute;
            var path = property.propertyPath;
            path = path[..(path.LastIndexOf('.') + 1)];
            var disabled = false;
            if (attribute.BooleanName != null)
            {
                var booleanPath = path + attribute.BooleanName;
                var boolean = property.serializedObject.FindProperty(booleanPath);
                disabled = !boolean.boolValue;
            }

            if (!disabled) return EditorGUI.GetPropertyHeight(property, true);
            return 0f;
        }
    }
}
