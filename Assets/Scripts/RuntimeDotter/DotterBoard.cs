using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeDotter
{
    public class DotterBoard
    {
        private readonly int[][] pixels;

        public Vector2Int Size { get; }
        public int PaletteSize { get; }

        public Rect Rect => new Rect(0, 0, Size.x, Size.y);
        public RectInt RectInt => new RectInt(Vector2Int.zero, Size);

        private static Color32[] buffer;

        public DotterBoard(Vector2Int size, int paletteSize)
        {
            pixels = new int[size.y][];
            for (int y = 0; y < size.y; y++)
            {
                pixels[y] = new int[size.x];
            }
            Size = size;
            PaletteSize = paletteSize;
        }

        public DotterBoard(DotterBoard board)
            : this(board.Size, board.PaletteSize)
        {
            board.CopyTo(this);
        }

        private DotterBoard() { }

        public int GetPixel(Vector2Int position)
        {
            if (!RectInt.Contains(position)) throw new System.ArgumentOutOfRangeException(nameof(position));

            return pixels[position.y][position.x];
        }

        public void SetPixel(Vector2Int position, int colorIndex)
        {
            if (!RectInt.Contains(position)) throw new System.ArgumentOutOfRangeException(nameof(position));
            if (colorIndex < 0 || PaletteSize <= colorIndex) throw new System.ArgumentOutOfRangeException(nameof(colorIndex));

            pixels[position.y][position.x] = colorIndex;
        }

        public void CopyTo(DotterBoard board)
        {
            if (board.Size != Size) throw new System.ArgumentException($"引数の {nameof(DotterBoard)} とサイズが一致しません。");
            if (board.PaletteSize != PaletteSize) throw new System.ArgumentException($"引数の {nameof(DotterBoard)} とパレットサイズが一致しません。");

            for (int y = 0; y < Size.y; y++)
            {
                for (int x = 0; x < Size.x; x++)
                {
                    board.pixels[y][x] = pixels[y][x];
                }
            }
        }

        public void SetPixelsTo(Texture2D texture, IReadOnlyList<ShiftableColor> palette)
        {
            if (palette.Count != PaletteSize) throw new System.ArgumentException(
                $"{nameof(palette)} の要素数 ({palette.Count}) が {nameof(PaletteSize)} ({PaletteSize}) と一致しません。");

            if (buffer == null || buffer.Length != texture.width * texture.height)
            {
                buffer = new Color32[texture.width * texture.height];
            }

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    var colorIndex = pixels[y * Size.y / texture.height][x * Size.x / texture.width];
                    buffer[x + y * texture.width] = palette[colorIndex].ToSpriteColor();
                }
            }
            texture.SetPixels32(buffer);
        }

        public void SetPixelsTo(Texture2D texture, System.ReadOnlySpan<ShiftableColor> palette)
        {
            if (palette.Length != PaletteSize) throw new System.ArgumentException(
                $"{nameof(palette)} の要素数 ({palette.Length}) が {nameof(PaletteSize)} ({PaletteSize}) と一致しません。");

            if (buffer == null || buffer.Length != texture.width * texture.height)
            {
                buffer = new Color32[texture.width * texture.height];
            }

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    var colorIndex = pixels[y * Size.y / texture.height][x * Size.x / texture.width];
                    buffer[x + y * texture.width] = palette[colorIndex].ToSpriteColor();
                }
            }
            texture.SetPixels32(buffer);
        }
    }
}
