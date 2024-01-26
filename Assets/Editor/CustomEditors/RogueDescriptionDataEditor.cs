using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace Roguegard.CharacterCreation.Editor
{
    /// <summary>
    /// Project ビューにアイコン（<see cref="RogueDescriptionData.Icon"/>）を表示させるエディタ拡張
    /// </summary>
    [CustomEditor(typeof(RogueDescriptionData), true)]
    public class RogueDescriptionDataEditor : UnityEditor.Editor
    {
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            var data = (RogueDescriptionData)target;
            var icon = data.Icon;
            if (icon == null) return base.RenderStaticPreview(assetPath, subAssets, width, height);

            var tempPreview = AssetPreview.GetAssetPreview(icon);
            if (tempPreview == null)
            {
                return base.RenderStaticPreview(assetPath, subAssets, width, height);
            }

            var size = new Vector2Int(width, height) / 2;
            var aspectRatio = icon.rect.width / icon.rect.height;
            if (icon.rect.width > icon.rect.height)
            {
                size.y = Mathf.FloorToInt(size.x / aspectRatio);
            }
            else if (icon.rect.width < icon.rect.height)
            {
                size.x = Mathf.FloorToInt(size.y * aspectRatio);
            }

            var preview = new Texture2D(size.x, size.y);
            var pixels = preview.GetPixels32();
            var scale = icon.rect.size / size;
            for (int i = 0; i < pixels.Length; i++)
            {
                var index = new Vector2(i % size.x, i / size.x);
                var position = index * scale;
                pixels[i] = tempPreview.GetPixel(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
            }
            preview.SetPixels32(pixels);
            preview.Apply();
            return preview;
        }
    }
}
