using System.Collections.Generic;
using UnityEngine;

namespace Lysionium.Views
{
    internal class RuntimeSpritePacker : System.IDisposable
    {
        private readonly Vector2Int packedSpriteSize;
        private readonly int cellCountX;
        private readonly int cellCountY;
        private readonly bool[][] filled;
        private readonly RenderTexture renderTexture;
        private readonly Sprite separatorSprite;
        private readonly int separatorWidth;

        public RuntimeSpritePacker(Vector2Int spriteSheetSize, Vector2Int packedSpriteSize, Sprite separatorSprite, int separatorWidth)
        {
            if (separatorWidth > packedSpriteSize.x) throw new System.ArgumentException(
                $"{nameof(separatorWidth)} は {nameof(packedSpriteSize)}.x 以下にしてください。");

            this.packedSpriteSize = packedSpriteSize;
            this.separatorSprite = separatorSprite;
            this.separatorWidth = separatorWidth;

            // スプライトを並べられる数を計算
            cellCountX = spriteSheetSize.x / packedSpriteSize.x;
            cellCountY = spriteSheetSize.y / packedSpriteSize.y;

            // フラグ配列を初期化
            filled = new bool[cellCountY][];
            for (int y = 0; y < cellCountY; y++)
            {
                filled[y] = new bool[cellCountX];
            }

            // RenderTexture を初期化
            renderTexture = RenderTexture.GetTemporary(spriteSheetSize.x, spriteSheetSize.y, 1);
            renderTexture.autoGenerateMips = false;
            renderTexture.filterMode = FilterMode.Point;
            RenderTexture.active = renderTexture;
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, spriteSheetSize.x, spriteSheetSize.y, 0);
            GL.Clear(true, true, Color.clear);
        }

        private bool CellIsFilled(int cellX, int cellY, Vector2Int span)
        {
            for (int y = 0; y < span.y; y++)
            {
                for (int x = 0; x < span.x; x++)
                {
                    if (filled[cellY + y][cellX + x]) return true;
                }
            }
            return false;
        }

        private void FillCells(int cellX, int cellY, Vector2Int span)
        {
            for (int y = 0; y < span.y; y++)
            {
                for (int x = 0; x < span.x; x++)
                {
                    filled[cellY + y][cellX + x] = true;
                }
            }
        }

        /// <summary>
        /// まだ埋まっていないセルを予約し、始点インデックスを取得する
        /// </summary>
        private Vector2Int ReserveEmptyIndex(Vector2Int span)
        {
            for (int y = 0; y <= cellCountY - span.y; y++)
            {
                for (int x = 0; x <= cellCountX - span.x; x++)
                {
                    if (!CellIsFilled(x, y, span))
                    {
                        FillCells(x, y, span);
                        return new Vector2Int(x, y);
                    }
                }
            }
            return -Vector2Int.one;
        }

        /// <summary>
        /// 指定のスプライトセットをパッキングし、パッキングされた位置を取得する
        /// </summary>
        /// <exception cref="System.InvalidOperationException"></exception>
        public Rect PackSpriteSet(IReadOnlyList<Sprite> spriteSet)
        {
            if (spriteSet.Count == 0) return Rect.zero;

            // スプライトの並べ方を計算
            var emptyIndex = ReserveEmptyIndex(new Vector2Int(spriteSet.Count, 1));
            if (emptyIndex.x == -1) throw new System.InvalidOperationException("キーバインドグリフの空き空間が見つかりませんでした。");

            // RenderTexture にスプライトを書き込む
            var position = emptyIndex * packedSpriteSize;
            for (int i = 0; i < spriteSet.Count; i++)
            {
                var sprite = spriteSet[i];
                var spriteTextureSize = new Vector2(sprite.texture.width, sprite.texture.height);
                var sourceRect = sprite.rect;
                sourceRect.position /= spriteTextureSize;
                sourceRect.size /= spriteTextureSize;

                if (sprite == separatorSprite)
                {
                    Graphics.DrawTexture(
                        new Rect(position + Vector2Int.left * (packedSpriteSize.x - separatorWidth) / 2, packedSpriteSize),
                        sprite.texture, sourceRect, 0, 0, 0, 0);
                    position.x += separatorWidth;
                }
                else
                {
                    Graphics.DrawTexture(new Rect(position, packedSpriteSize), sprite.texture, sourceRect, 0, 0, 0, 0);

                    position.x += packedSpriteSize.x;
                }
            }
            {
                // 書き込んだ位置を返す
                var startPosition = emptyIndex * packedSpriteSize;
                var size = new Vector2Int(position.x - startPosition.x, packedSpriteSize.y);
                return new Rect(startPosition, size);
            }
        }

        /// <summary>
        /// パッキング結果を指定のテクスチャに書き込む
        /// </summary>
        public void WriteTo(Texture2D spriteSheet)
        {
            spriteSheet.ReadPixels(new Rect(0f, 0f, spriteSheet.width, spriteSheet.height), 0, 0);
        }

        public void Dispose()
        {
            GL.PopMatrix();
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(renderTexture);
        }
    }
}
