using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace OchalikeSprites.Editor
{
    [CustomEditor(typeof(SpriteMotionData), true)]
    [CanEditMultipleObjects]
    public class SpriteMotionDataEditor : UnityEditor.Editor
    {
        private RenderTexture preview;

        protected virtual void OnEnable()
        {
            preview = RenderTexture.GetTemporary(128, 128, 1);
            preview.autoGenerateMips = false;
            preview.filterMode = FilterMode.Point;
            UpdatePreview();

            OchalikeSpritePreview.Primary.OnUpdatePreview += UpdatePreview;
        }

        protected virtual void OnDisable()
        {
            if (preview != null) { RenderTexture.ReleaseTemporary(preview); }

            OchalikeSpritePreview.Primary.OnUpdatePreview -= UpdatePreview;
        }

        private void UpdatePreview()
        {
            var data = (SpriteMotionData)target;
            OchalikeSpritePreview.Primary.RenderTo(
                preview,
                step2GetOchalikeSprite: (ochalikeSpriteData, bareColor, morph) =>
                {
                    if (ochalikeSpriteData == null) { ochalikeSpriteData = OchalikeSpritePreview.GetOchalikeSpriteData(data); }
                    if (ochalikeSpriteData == null) return OchalikeBone.CreateClearOchalikeSprite(bareColor);
                    else return ochalikeSpriteData.CreateBoneWithHairColor(bareColor, morph);
                },
                step3GetSpriteTransform: (__, direction, animationTime) =>
                {
                    var spriteTransform = OchalikeSpriteTransform.Identity;
                    data.ApplyTo(animationTime, direction, ref spriteTransform, out _);
                    return spriteTransform;
                },
                step4Render: (preview, ochalikeSprite, morph, spriteTransform, defaultColor) =>
                {
                    var renderController = new OchalikeTextureRenderController();
                    renderController.Set(ochalikeSprite, morph, spriteTransform, defaultColor);
                    renderController.PositionOffset = new Vector3(0f, -0.25f);
                    renderController.ScaleOffset = Vector3.one / 2f;
                    renderController.RenderTo(preview);
                });
        }

        public override bool HasPreviewGUI() => targets.Length == 1;
        public override bool RequiresConstantRepaint() => OchalikeSpritePreview.Primary.IsPlaying;
        public override void OnPreviewGUI(Rect r, GUIStyle background) => OchalikeSpritePreview.Primary.OnPreviewGUI(r, preview);
        public override void OnPreviewSettings() => OchalikeSpritePreview.Primary.OnPreviewSettings(enableMotionData: false);
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
            => OchalikeSpritePreview.Primary.RenderStaticPreview(width, height, preview);
    }
}
