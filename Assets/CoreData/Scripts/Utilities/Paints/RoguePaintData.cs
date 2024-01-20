using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class RoguePaintData
    {
        private int[][] board;

        public static int BoardSize => 32;
        public static int PaletteSize => 16;

        private static readonly RectInt rectInt = new RectInt(0, 0, BoardSize, BoardSize);
        private static readonly Rect rect = new Rect(0, 0, BoardSize, BoardSize);
        private static Color32[] pixels;

        public RoguePaintData()
        {
            board = new int[BoardSize][];
            for (int y = 0; y < BoardSize; y++)
            {
                board[y] = new int[BoardSize];
            }
        }

        [ObjectFormer.CreateInstance]
        private RoguePaintData(bool dummy) { }

        public int GetPixel(Vector2Int position)
        {
            if (!rectInt.Contains(position)) throw new System.ArgumentOutOfRangeException(nameof(position));

            return board[position.y][position.x];
        }

        public void SetPixel(Vector2Int position, int colorIndex)
        {
            if (!rectInt.Contains(position)) throw new System.ArgumentOutOfRangeException(nameof(position));
            if (colorIndex < 0 || PaletteSize <= colorIndex) throw new System.ArgumentOutOfRangeException(nameof(colorIndex));

            board[position.y][position.x] = colorIndex;
        }

        public void SetPixelsTo(Texture2D texture, Spanning<RoguePaintColor> palette)
        {
            if (pixels == null || pixels.Length < texture.width * texture.height)
            {
                pixels = new Color32[texture.width * texture.height];
            }

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    var colorIndex = board[y * BoardSize / texture.height][x * BoardSize / texture.width];
                    pixels[x + y * texture.width] = palette[colorIndex].ToColor();
                }
            }
            texture.SetPixels32(pixels);
        }

        public Sprite ToSprite(Spanning<RoguePaintColor> palette)
        {
            var texture = new Texture2D(BoardSize, BoardSize);
            SetPixelsTo(texture, palette);
            texture.Apply();
            var sprite = Sprite.Create(texture, rect, new Vector2(.5f, .5f), RoguegardSettings.PixelsPerUnit);
            return sprite;
        }

        public void ToSprite(Spanning<RoguePaintColor> palette, Vector2 upperPivot, Vector2 lowerPivot, out Sprite upperSprite, out Sprite lowerSprite)
        {
            var texture = new Texture2D(BoardSize, BoardSize);
            texture.filterMode = FilterMode.Point;
            SetPixelsTo(texture, palette);
            texture.Apply();
            var splitPosition = BoardSize / 2;
            var upperRect = new Rect(0f, splitPosition, BoardSize, BoardSize - splitPosition);
            upperSprite = Sprite.Create(texture, upperRect, upperPivot, RoguegardSettings.PixelsPerUnit);
            var lowerRect = new Rect(0f, 0f, BoardSize, splitPosition);
            lowerSprite = Sprite.Create(texture, lowerRect, lowerPivot, RoguegardSettings.PixelsPerUnit);
        }
    }
}
