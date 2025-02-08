using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace ListingMF.Editor
{
    public abstract class ScriptableGenerator : ScriptableObject
    {
        protected virtual string IconSearchFilter => null;

        public abstract void Generate();

        [CustomEditor(typeof(ScriptableGenerator), true)]
        [CanEditMultipleObjects]
        protected class ScriptableGeneratorEditor : UnityEditor.Editor
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

            public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
            {
                var iconSearchFilter = ((ScriptableGenerator)target).IconSearchFilter;
                if (iconSearchFilter != null)
                {
                    var iconGuids = AssetDatabase.FindAssets($"{iconSearchFilter} t:sprite", new[] { "Assets", "Packages" });
                    foreach (var iconGuid in iconGuids)
                    {
                        var iconPath = AssetDatabase.GUIDToAssetPath(iconGuid);
                        var icon = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
                        if (icon == null) continue;

                        var tempPreview = AssetPreview.GetAssetPreview(icon);
                        if (tempPreview == null) continue;

                        var preview = new Texture2D(width, height);
                        EditorUtility.CopySerialized(tempPreview, preview);
                        return preview;
                    }
                }

                return base.RenderStaticPreview(assetPath, subAssets, width, height);
            }
        }
    }
}
