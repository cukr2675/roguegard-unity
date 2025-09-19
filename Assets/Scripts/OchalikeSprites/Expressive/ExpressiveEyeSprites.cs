using UnityEngine;

namespace OchalikeSprites
{
    /// <summary>
    /// 目の表情差分を自動作成するクラス
    /// </summary>
    internal class ExpressiveEyeSprite : System.IDisposable
    {
        public Sprite Neutral { get; }
        public Sprite Angry { get; }
        public Sprite Crying { get; }
        public Sprite Droopy { get; }
        public Sprite Sn { get; }

        public ExpressiveEyeSprite(Sprite neutral)
        {
            Neutral = neutral;

            var x = Mathf.FloorToInt(neutral.rect.x);
            var y = Mathf.FloorToInt(neutral.rect.y);
            var width = Mathf.FloorToInt(neutral.rect.width);
            var height = Mathf.FloorToInt(neutral.rect.height);
            var pivotX = Mathf.FloorToInt(neutral.pivot.x);
            var pivotY = Mathf.FloorToInt(neutral.pivot.y);
            if (neutral.rect.x - x != 0 || neutral.rect.y - y != 0 ||
                neutral.rect.width - width != 0 || neutral.rect.height - height != 0 ||
                neutral.pivot.x - pivotX != 0 || neutral.pivot.y - pivotY != 0)
            {
                Debug.LogWarning(
                    $"{nameof(ExpressiveEyeSprite)} で使用するスプライトのサイズが整数ではありません。正常に生成されない可能性があります。 " +
                    $"(rect: {neutral.rect}, pivot: {neutral.pivot} (of {neutral.name})");
            }

            var texture = new Texture2D(width * 4, height, TextureFormat.RGBA32, false);
            var colors = neutral.texture.GetPixels32();
            {
                texture.SetPixels32(width * 0, 0, width, height, colors);
                Angry = Sprite.Create(texture, new Rect(width * 0f, 0f, width, height), new Vector2(pivotX, pivotY));
            }
            {
                texture.SetPixels32(width * 1, 0, width, height, colors);
                Crying = Sprite.Create(texture, new Rect(width * 1f, 0f, width, height), new Vector2(pivotX, pivotY));
            }
            {
                texture.SetPixels32(width * 2, 0, width, height, colors);
                Droopy = Sprite.Create(texture, new Rect(width * 2f, 0f, width, height), new Vector2(pivotX, pivotY));
            }
        }

        public void Dispose()
        {
            var texture = Angry.texture;
            Object.Destroy(Angry);
            Object.Destroy(Crying);
            Object.Destroy(Droopy);
            Object.Destroy(Sn);
            Object.Destroy(texture);
        }
    }
}
