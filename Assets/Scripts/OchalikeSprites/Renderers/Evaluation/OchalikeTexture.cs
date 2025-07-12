using UnityEngine;

namespace OchalikeSprites
{
    public static class OchalikeTexture
    {
        public static Texture2D Create(int width, int height, OchalikeTextureRenderController renderController)
        {
            // RenderTexture を初期化
            var renderTexture = RenderTexture.GetTemporary(width, height, 1);
            renderTexture.autoGenerateMips = false;
            renderTexture.filterMode = FilterMode.Point;

            try
            {
                // RenderController を通して RenderTexture に書き込む
                renderController.RenderTo(renderTexture);

                // RenderTexture をアンチエイリアスなしで Texture2D に写す
                RenderTexture.active = renderTexture;
                var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
                texture.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
                return texture;
            }
            finally
            {
                RenderTexture.active = null;
                RenderTexture.ReleaseTemporary(renderTexture);
            }
        }

        public static void ScalingCopy(Texture source, Texture2D dest)
        {
            // 書き込み済み RenderTexture からプレビューサイズの RenderTexture へスケーリングしてコピー
            var scaledRenderTexture = RenderTexture.GetTemporary(dest.width, dest.height, 1);
            scaledRenderTexture.autoGenerateMips = false;
            scaledRenderTexture.filterMode = FilterMode.Point;

            try
            {
                Graphics.Blit(source, scaledRenderTexture);

                // プレビューサイズの RenderTexture を Texture2D に写す
                RenderTexture.active = scaledRenderTexture;
                dest.ReadPixels(new Rect(0f, 0f, dest.width, dest.height), 0, 0);
            }
            finally
            {
                RenderTexture.active = null;
                RenderTexture.ReleaseTemporary(scaledRenderTexture);
            }
        }
    }
}
