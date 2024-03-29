using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.IO;
using UnityEditor;
using UnityEditor.U2D.Sprites;

namespace Roguegard.Editor
{
    [CreateAssetMenu(menuName = "RoguegardData/Editor/Slice20RuleTileGenerator")]
    public class Slice20RuleTileGenerator : ScriptableGenerator
    {
        [SerializeField] private Texture2D _source = null;
        [SerializeField] private RuleTile _ruleTile = null;
        [SerializeField] private int _pixelsPerUnit = 32;

        private Color32[] pixels;

        // 18 19 20 21 22 23
        // 12       15 16 17
        //  6        9 10 11
        //  0  1  2  3  4  5

        private static readonly Slicer[] slicers = new Slicer[]
        {
            new() { X = 0, Y = 3, UpperLeft = 18, UpperRight = 19, LowerLeft = 12, LowerRight = 17 }, // 0
            new() { X = 1, Y = 3, UpperLeft = 20, UpperRight = 19, LowerLeft = 16, LowerRight = 17 },
            new() { X = 2, Y = 3, UpperLeft = 20, UpperRight = 21, LowerLeft = 16, LowerRight = 15 },

            new() { X = 0, Y = 2, UpperLeft =  6, UpperRight = 23, LowerLeft = 12, LowerRight = 17 }, // 3
            new() { X = 1, Y = 2, UpperLeft = 22, UpperRight = 23, LowerLeft = 16, LowerRight = 17 },
            new() { X = 2, Y = 2, UpperLeft = 22, UpperRight =  9, LowerLeft = 16, LowerRight = 15 },

            new() { X = 0, Y = 1, UpperLeft =  6, UpperRight = 23, LowerLeft =  0, LowerRight =  1 }, // 6
            new() { X = 1, Y = 1, UpperLeft = 22, UpperRight = 23, LowerLeft =  2, LowerRight =  1 },
            new() { X = 2, Y = 1, UpperLeft = 22, UpperRight =  9, LowerLeft =  2, LowerRight =  3 },

            new() { X = 3, Y = 3, UpperLeft = 18, UpperRight = 21, LowerLeft = 12, LowerRight = 15 }, // 9
            new() { X = 3, Y = 2, UpperLeft =  6, UpperRight =  9, LowerLeft = 12, LowerRight = 15 },
            new() { X = 3, Y = 1, UpperLeft =  6, UpperRight =  9, LowerLeft =  0, LowerRight =  3 },

            new() { X = 0, Y = 0, UpperLeft = 18, UpperRight = 19, LowerLeft =  0, LowerRight =  1 }, // 12
            new() { X = 1, Y = 0, UpperLeft = 20, UpperRight = 19, LowerLeft =  2, LowerRight =  1 },
            new() { X = 2, Y = 0, UpperLeft = 20, UpperRight = 21, LowerLeft =  2, LowerRight =  3 },

            new() { X = 3, Y = 0, UpperLeft = 18, UpperRight = 21, LowerLeft =  0, LowerRight =  3 }, // 15



            new() { X = 4, Y = 3, UpperLeft = 22, UpperRight = 23, LowerLeft = 16, LowerRight =  5 }, // 16
            new() { X = 5, Y = 3, UpperLeft = 22, UpperRight = 23, LowerLeft =  4, LowerRight = 17 },
            new() { X = 4, Y = 2, UpperLeft = 22, UpperRight = 11, LowerLeft = 16, LowerRight = 17 },
            new() { X = 5, Y = 2, UpperLeft = 10, UpperRight = 23, LowerLeft = 16, LowerRight = 17 },

            new() { X = 4, Y = 1, UpperLeft = 22, UpperRight = 11, LowerLeft = 16, LowerRight =  5 }, // 20
            new() { X = 5, Y = 1, UpperLeft = 22, UpperRight = 23, LowerLeft =  4, LowerRight =  5 },
            new() { X = 4, Y = 0, UpperLeft = 10, UpperRight = 11, LowerLeft = 16, LowerRight = 17 },
            new() { X = 5, Y = 0, UpperLeft = 10, UpperRight = 23, LowerLeft =  4, LowerRight = 17 },

            new() { X = 6, Y = 3, UpperLeft = 22, UpperRight = 11, LowerLeft =  4, LowerRight =  5 }, // 24
            new() { X = 7, Y = 3, UpperLeft = 10, UpperRight = 23, LowerLeft =  4, LowerRight =  5 },
            new() { X = 6, Y = 2, UpperLeft = 10, UpperRight = 11, LowerLeft = 16, LowerRight =  5 },
            new() { X = 7, Y = 2, UpperLeft = 10, UpperRight = 11, LowerLeft =  4, LowerRight = 17 },



            new() { X = 6, Y = 1, UpperLeft = 18, UpperRight = 19, LowerLeft = 12, LowerRight =  5 }, // 28
            new() { X = 7, Y = 1, UpperLeft = 20, UpperRight = 21, LowerLeft =  4, LowerRight = 15 },
            new() { X = 6, Y = 0, UpperLeft =  6, UpperRight = 11, LowerLeft =  0, LowerRight =  1 },
            new() { X = 7, Y = 0, UpperLeft = 10, UpperRight =  9, LowerLeft =  2, LowerRight =  3 },

            new() { X = 8, Y = 3, UpperLeft =  6, UpperRight = 23, LowerLeft = 12, LowerRight =  5 }, // 32
            new() { X = 9, Y = 3, UpperLeft = 22, UpperRight =  9, LowerLeft =  4, LowerRight = 15 },
            new() { X = 8, Y = 2, UpperLeft =  6, UpperRight = 11, LowerLeft = 12, LowerRight = 17 },
            new() { X = 9, Y = 2, UpperLeft = 10, UpperRight =  9, LowerLeft = 16, LowerRight = 15 },

            new() { X = 8, Y = 1, UpperLeft = 20, UpperRight = 19, LowerLeft = 16, LowerRight =  5 }, // 36
            new() { X = 9, Y = 1, UpperLeft = 20, UpperRight = 19, LowerLeft =  4, LowerRight = 17 },
            new() { X = 8, Y = 0, UpperLeft = 22, UpperRight = 11, LowerLeft =  2, LowerRight =  1 },
            new() { X = 9, Y = 0, UpperLeft = 10, UpperRight = 23, LowerLeft =  2, LowerRight =  1 },

            new() { X = 10, Y = 3, UpperLeft =  6, UpperRight = 11, LowerLeft = 12, LowerRight =  5 }, // 40
            new() { X = 11, Y = 3, UpperLeft = 10, UpperRight =  9, LowerLeft =  4, LowerRight = 15 },
            new() { X = 10, Y = 2, UpperLeft = 10, UpperRight = 11, LowerLeft =  2, LowerRight =  1 },
            new() { X = 11, Y = 2, UpperLeft = 20, UpperRight = 19, LowerLeft =  4, LowerRight =  5 },



            new() { X = 10, Y = 1, UpperLeft = 10, UpperRight = 23, LowerLeft = 16, LowerRight =  5 }, // 44
            new() { X = 11, Y = 1, UpperLeft = 22, UpperRight = 11, LowerLeft =  4, LowerRight = 17 },
            new() { X = 10, Y = 0, UpperLeft = 10, UpperRight = 11, LowerLeft =  4, LowerRight =  5 },
        };

        private static readonly string[] neighbors = new[]
        {
            @" .x.
               x@o
               .oo ", // 0
            @" .x.
               o@o
               ooo ",
            @" .x.
               o@x
               oo. ",

            @" .oo
               x@o
               .oo ", // 3
            @" ooo
               o@o
               ooo ",
            @" oo.
               o@x
               oo. ",

            @" .oo
               x@o
               .x. ", // 6
            @" ooo
               o@o
               .x. ",
            @" oo.
               o@x
               .x. ",

            @" .x.
               x@x
               .o. ", // 9
            @" .o.
               x@x
               .o. ",
            @" .o.
               x@x
               .x. ",

            @" .x.
               x@o
               .x. ", // 12
            @" .x.
               o@o
               .x. ",
            @" .x.
               o@x
               .x. ",

            @" .x.
               x@x
               .x. ", // 15


            
            @" ooo
               o@o
               oox ", // 16
            @" ooo
               o@o
               xoo ",
            @" oox
               o@o
               ooo ",
            @" xoo
               o@o
               ooo ",

            @" oox
               o@o
               oox ", // 20
            @" ooo
               o@o
               xox ",
            @" xox
               o@o
               ooo ",
            @" xoo
               o@o
               xoo ",

            @" oox
               o@o
               xox ", // 24
            @" xoo
               o@o
               xox ",
            @" xox
               o@o
               oox ",
            @" xox
               o@o
               xoo ",



            @" .x.
               x@o
               .ox ", // 28
            @" .x.
               o@x
               xo. ",
            @" .ox
               x@o
               .x. ",
            @" xo.
               o@x
               .x. ",

            @" .oo
               x@o
               .ox ", // 32
            @" oo.
               o@x
               xo. ",
            @" .ox
               x@o
               .oo ",
            @" xo.
               o@x
               oo. ",

            @" .x.
               o@o
               oox ", // 36
            @" .x.
               o@o
               xoo ",
            @" oox
               o@o
               .x. ",
            @" xoo
               o@o
               .x. ",

            @" .ox
               x@o
               .ox ", // 40
            @" xo.
               o@x
               xo. ",
            @" xox
               o@o
               .x. ",
            @" .x.
               o@o
               xox ",



            @" xoo
               o@o
               oox ", // 44
            @" oox
               o@o
               xoo ",
            @" xox
               o@o
               xox ",
        };

        static Slice20RuleTileGenerator()
        {
            for (int i = 0; i < slicers.Length; i++)
            {
                slicers[i].Index = i;
                slicers[i].Neighbors = GetNeighbors(neighbors[i]);
            }
        }

        private static Dictionary<Vector3Int, int> GetNeighbors(string str)
        {
            str = str.Replace(" ", "").Replace("\n", "").Replace("\r", "");
            if (str.Length != 9) throw new RogueException(str);

            var neighbors = new Dictionary<Vector3Int, int>();
            Add(-1, +1, str[0]);
            Add(+0, +1, str[1]);
            Add(+1, +1, str[2]);
            Add(-1, +0, str[3]);
            Add(+1, +0, str[5]);
            Add(-1, -1, str[6]);
            Add(+0, -1, str[7]);
            Add(+1, -1, str[8]);
            return neighbors;

            void Add(int x, int y, char item)
            {
                if (item == 'x') { neighbors.Add(new Vector3Int(x, y), RuleTile.TilingRuleOutput.Neighbor.NotThis); }
                else if (item == 'o') { neighbors.Add(new Vector3Int(x, y), RuleTile.TilingRuleOutput.Neighbor.This); }
            }
        }

        protected override void Generate()
        {
            if (_source.width != _pixelsPerUnit * 3 || _source.height != _pixelsPerUnit * 2) throw new RogueException(
                $"テクスチャのサイズが {_pixelsPerUnit * 3}x{_pixelsPerUnit * 2} ではありません。");

            // source を 24 分割したスプライトを生成する
            var sprites = new Sprite[24];
            var spriteSize = _pixelsPerUnit / 2f;
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 6; x++)
                {
                    var rect = new Rect(spriteSize * x, spriteSize * y, spriteSize, spriteSize);
                    var sprite = Sprite.Create(_source, rect, Vector2.zero);
                    sprites[x + y * 6] = sprite;
                }
            }

            if (pixels == null || pixels.Length != _pixelsPerUnit * _pixelsPerUnit)
            {
                pixels = new Color32[_pixelsPerUnit * _pixelsPerUnit];
            }
            var texture = new Texture2D(_pixelsPerUnit * 12, _pixelsPerUnit * 4, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;

            foreach (var slicer in slicers)
            {
                // pixels へ書き込み
                slicer.GetPixelsNonAlloc(sprites, pixels, _pixelsPerUnit);

                // テクスチャへ書き込み
                var x = slicer.X * _pixelsPerUnit;
                var y = slicer.Y * _pixelsPerUnit;
                texture.SetPixels32(x, y, _pixelsPerUnit, _pixelsPerUnit, pixels);
            }

            // png 画像として保存
            var ruleTilePath = AssetDatabase.GetAssetPath(_ruleTile);
            var ruleTileDirectory = Path.GetDirectoryName(ruleTilePath);
            var ruleTileTexturePath = $@"{ruleTileDirectory}\{_ruleTile.name}.png";
            var png = texture.EncodeToPNG();
            File.WriteAllBytes(ruleTileTexturePath, png);
            AssetDatabase.ImportAsset(ruleTileTexturePath);

            // 保存した画像のインポート設定を取得
            var ruleTileTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(ruleTileTexturePath);
            var importer = (TextureImporter)AssetImporter.GetAtPath(ruleTileTexturePath);
            importer.spritePixelsPerUnit = _pixelsPerUnit;
            importer.spriteImportMode = SpriteImportMode.Multiple;

            // 保存した画像にスプライトを設定
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var provider = factory.GetSpriteEditorDataProviderFromObject(importer);
            provider.InitSpriteEditorDataProvider();
            provider.SetSpriteRects(slicers.Select(x => x.GetSpriteRect(_ruleTile.name, _pixelsPerUnit)).ToArray());
            provider.Apply();
            EditorUtility.SetDirty(ruleTileTexture);
            AssetDatabase.ImportAsset(ruleTileTexturePath);

            // RuleTile を更新
            _ruleTile.m_TilingRules.Clear();
            _ruleTile.m_TilingRules.AddRange(slicers.Select(x => x.GetTilingRule(_ruleTile.name)));
            EditorUtility.SetDirty(_ruleTile);
            AssetDatabase.ImportAsset(ruleTilePath);
        }

        private class Slicer
        {
            public int Index { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public int UpperLeft { get; set; }
            public int UpperRight { get; set; }
            public int LowerLeft { get; set; }
            public int LowerRight { get; set; }
            public Dictionary<Vector3Int, int> Neighbors { get; set; }

            public void GetPixelsNonAlloc(Sprite[] sprites, Color32[] pixels, int pixelsPerUnit)
            {
                var size = pixelsPerUnit / 2;
                GetPixelsNonAlloc(sprites[UpperLeft], pixels, pixelsPerUnit, size, 0, size);
                GetPixelsNonAlloc(sprites[UpperRight], pixels, pixelsPerUnit, size, size, size);
                GetPixelsNonAlloc(sprites[LowerLeft], pixels, pixelsPerUnit, size, 0, 0);
                GetPixelsNonAlloc(sprites[LowerRight], pixels, pixelsPerUnit, size, size, 0);
            }

            private static void GetPixelsNonAlloc(Sprite sprite, Color32[] pixels, int pixelsPerUnit, int size, int startX, int startY)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        var point = startX + x + (startY + y) * pixelsPerUnit;
                        var rect = sprite.rect;
                        pixels[point] = sprite.texture.GetPixel((int)rect.x + x, (int)rect.y + y);
                    }
                }
            }

            public SpriteRect GetSpriteRect(string name, int pixelsPerUnit)
            {
                var spriteRect = new SpriteRect();
                spriteRect.name = $"{name}_{Index}";
                var x = X * pixelsPerUnit;
                var y = Y * pixelsPerUnit;
                spriteRect.rect = Rect.MinMaxRect(x, y, x + pixelsPerUnit, y + pixelsPerUnit);
                spriteRect.alignment = SpriteAlignment.Center;
                return spriteRect;
            }

            public RuleTile.TilingRule GetTilingRule(string name)
            {
                var sprite = RoguegardAssetDatabase.GetSprite($"{name}_{Index}");
                var tilingRule = new RuleTile.TilingRule();
                tilingRule.m_Sprites = new[] { sprite };
                tilingRule.ApplyNeighbors(Neighbors);
                return tilingRule;
            }
        }
    }
}
