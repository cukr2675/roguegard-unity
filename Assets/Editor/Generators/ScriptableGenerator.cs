using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace Roguegard.Editor
{
    public abstract class ScriptableGenerator : ScriptableObject
    {
        protected abstract void Generate();

        [CustomEditor(typeof(ScriptableGenerator), true)]
        [CanEditMultipleObjects]
        private class Editor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (GUILayout.Button("Generate"))
                {
                    foreach (var target in targets)
                    {
                        ((ScriptableGenerator)target).Generate();
                    }
                }
            }
        }
    }
}
