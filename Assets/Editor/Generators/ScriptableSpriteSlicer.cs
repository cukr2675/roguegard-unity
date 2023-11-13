using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Roguegard.Editor
{
    [CreateAssetMenu(menuName = "RoguegardData/Editor/SpriteSlicer")]
    public class ScriptableSpriteSlicer : ScriptableObject
    {
        [SerializeField] private Texture2D _targetTexture = null;
        public Texture2D TargetTexture { get => _targetTexture; set => _targetTexture = value; }

        [SerializeField] private List<Slicer> _slicers = null;
        public List<Slicer> Slicers { get => _slicers; set => _slicers = value; }

        private static readonly List<string> formats = new List<string>();

        private void StartSlice()
        {
            var path = AssetDatabase.GetAssetPath(_targetTexture);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.spritesheet = Slicers.SelectMany(x => x.Select(i => i)).ToArray();
            EditorUtility.SetDirty(_targetTexture);
            AssetDatabase.ImportAsset(path);
        }

        private void ToSingle()
        {
            var path = AssetDatabase.GetAssetPath(_targetTexture);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.spritesheet = new[] { new SpriteMetaData() };
            EditorUtility.SetDirty(_targetTexture);
            AssetDatabase.ImportAsset(path);
        }

        private void OnValidate()
        {
            foreach (var slicer in Slicers)
            {
                slicer.Validate();
            }

            // Format を一意にする
            formats.Clear();
            foreach (var slicer in Slicers)
            {
                if (formats.Contains(slicer.Format))
                {
                    slicer.Format = ObjectNames.GetUniqueName(formats.ToArray(), slicer.Format);
                }
                formats.Add(slicer.Format);
            }
        }

        [System.Serializable]
        public class Slicer : IEnumerable<SpriteMetaData>
        {
            [SerializeField] private string _format;
            public string Format { get => _format; set => _format = value; }

            [SerializeField] private RectInt _rect;
            public RectInt Rect { get => _rect; set => _rect = value; }

            [SerializeField] private Vector2Int _pixelSize;
            public Vector2Int PixelSize { get => _pixelSize; set => _pixelSize = value; }

            [SerializeField] private Vector2Int _padding;
            public Vector2Int Padding { get => _padding; set => _padding = value; }

            [SerializeField] private Vector2Int _pixelPivot;
            public Vector2Int PixelPivot { get => _pixelPivot; set => _pixelPivot = value; }

            private static readonly char[] alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            private const float pixelPerUnits = 32;

            public void Validate()
            {
                _pixelSize.x = Mathf.Clamp(PixelSize.x, 1, int.MaxValue);
                _pixelSize.y = Mathf.Clamp(PixelSize.y, 1, int.MaxValue);
            }

            public IEnumerator<SpriteMetaData> GetEnumerator()
            {
                if (PixelSize.x <= 0 || PixelSize.y <= 0) throw new RogueException();

                // ループは x -> y の順番
                var pivot = new Vector2(PixelPivot.x, PixelPivot.y) / pixelPerUnits; ;
                var index = Vector2Int.zero;
                for (int x = Rect.xMin; x + PixelSize.x <= Rect.xMax; x += PixelSize.x + Padding.x) // x 軸は左から右へ
                {
                    index.y = 0;
                    for (int y = Rect.yMax; y - PixelSize.y >= Rect.yMin; y -= PixelSize.y + Padding.y) // y 軸は上から下へ
                    {
                        var sprite = new SpriteMetaData();
                        sprite.name = string.Format(Format, alphabets[index.x], index.y);
                        sprite.rect = UnityEngine.Rect.MinMaxRect(x, y - PixelSize.y, x + PixelSize.x, y);
                        sprite.alignment = (int)SpriteAlignment.Custom;
                        sprite.pivot = pivot;
                        yield return sprite;

                        index.y++;
                    }
                    index.x++;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [CustomEditor(typeof(ScriptableSpriteSlicer))]
        [CanEditMultipleObjects]
        private class ScriptableSpriteSlicerEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (GUILayout.Button("Slice"))
                {
                    foreach (var target in targets)
                    {
                        ((ScriptableSpriteSlicer)target).StartSlice();
                    }
                }
                EditorGUILayout.Space();

                var rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.helpBox);
                var helpBoxRect = rect;
                helpBoxRect.height *= 5;
                EditorGUI.HelpBox(
                    helpBoxRect,
                    "警告 (Identifier uniqueness violation) が発生する場合は以下の操作を行う\n" +
                    "1. 下の To Single ボタンを押す\n" +
                    "2. 対象テクスチャの SpriteEditor を開いて Delete キーを押す\n" +
                    "3. SpriteEditor の Apply を押す", MessageType.Info);

                var buttonRect = rect;
                buttonRect.y += helpBoxRect.height;
                if (EditorGUI.LinkButton(buttonRect, "To Single"))
                {
                    foreach (var target in targets)
                    {
                        ((ScriptableSpriteSlicer)target).ToSingle();
                    }
                }
            }
        }
    }
}
