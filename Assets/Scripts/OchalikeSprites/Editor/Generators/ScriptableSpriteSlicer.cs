using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace OchalikeSprites.Editor
{
    [CreateAssetMenu(menuName = "Ochalike Sprites/Editor/Sprite Slicer")]
    public class ScriptableSpriteSlicer : ScriptableObject
    {
        [SerializeField] private Texture2D _targetTexture = null;
        public Texture2D TargetTexture { get => _targetTexture; set => _targetTexture = value; }

        [SerializeField] private string _formatPrefix = null;
        public string FormatPrefix { get => _formatPrefix; set => _formatPrefix = value; }

        [SerializeField] private string _formatSuffix = null;
        public string FormatSuffix { get => _formatSuffix; set => _formatSuffix = value; }

        [SerializeField] private List<Slicer> _slicers = null;
        public List<Slicer> Slicers { get => _slicers; set => _slicers = value; }

        private static readonly List<string> formats = new();

        private void StartSlice()
        {
            var path = AssetDatabase.GetAssetPath(_targetTexture);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.spriteImportMode = SpriteImportMode.Multiple;

            var factory = new SpriteDataProviderFactories();
            factory.Init();

            var provider = factory.GetSpriteEditorDataProviderFromObject(importer);
            provider.InitSpriteEditorDataProvider();
            provider.SetSpriteRects(Slicers.SelectMany(x => x.ToSpriteRects(FormatPrefix, FormatSuffix)).ToArray());
            provider.Apply();

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
        public class Slicer
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

            public void Validate()
            {
                _pixelSize.x = Mathf.Clamp(PixelSize.x, 1, int.MaxValue);
                _pixelSize.y = Mathf.Clamp(PixelSize.y, 1, int.MaxValue);
            }

            public IEnumerable<SpriteRect> ToSpriteRects(string formatPrefix, string formatSuffix)
            {
                if (PixelSize.x <= 0 || PixelSize.y <= 0) throw new System.InvalidOperationException();

                // ループは x -> y の順番
                var pivot = new Vector2(PixelPivot.x, PixelPivot.y) / _pixelSize;
                var index = Vector2Int.zero;
                for (int x = Rect.xMin; x + PixelSize.x <= Rect.xMax; x += PixelSize.x + Padding.x) // x 軸は左から右へ
                {
                    index.y = 0;
                    for (int y = Rect.yMax; y - PixelSize.y >= Rect.yMin; y -= PixelSize.y + Padding.y) // y 軸は上から下へ (画像編集ソフトに合わせた方向)
                    {
                        yield return new SpriteRect
                        {
                            name = string.Format(formatPrefix + Format + formatSuffix, alphabets[index.x], index.y),
                            rect = UnityEngine.Rect.MinMaxRect(x, y - PixelSize.y, x + PixelSize.x, y),
                            alignment = SpriteAlignment.Custom,
                            pivot = pivot
                        };
                        index.y++;
                    }
                    index.x++;
                }
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
            }
        }
    }
}
