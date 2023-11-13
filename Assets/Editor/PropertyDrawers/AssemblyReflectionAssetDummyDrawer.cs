using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace Roguegard.Editor
{
    /// <summary>
    /// <see cref="ReferableScript.AssemblyReflectionAssetDummyAttribute"/> を非表示にする。
    /// </summary>
    [CustomPropertyDrawer(typeof(ReferableScript.AssemblyReflectionAssetDummyAttribute))]
    public class AssemblyReflectionAssetDummyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0f;
        }
    }
}
