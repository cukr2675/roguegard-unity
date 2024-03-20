using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace Roguegard.Editor
{
    [CustomPropertyDrawer(typeof(EnabledByAttribute))]
    public class EnabledByDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = (EnabledByAttribute)this.attribute;
            var path = property.propertyPath;
            path = path.Substring(0, path.LastIndexOf('.') + 1);
            var booleanPath = path + attribute.BooleanName;
            var boolean = property.serializedObject.FindProperty(booleanPath);
            var disabled = !boolean.boolValue;

            if (disabled) { EditorGUI.BeginDisabledGroup(true); }

            EditorGUI.PropertyField(position, property, true);

            if (disabled) { EditorGUI.EndDisabledGroup(); }
        }
    }
}
