using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace OchalikeSprites.Editor
{
    [CustomEditor(typeof(OchalikeMorphData), true)]
    [CanEditMultipleObjects]
    public class OchalikeMorphDataEditor : UnityEditor.Editor
    {
        private RenderTexture preview;

        protected virtual void OnEnable()
        {
            preview = RenderTexture.GetTemporary(64, 64, 1);
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
            var data = (OchalikeMorphData)target;
            OchalikeSpritePreview.Primary.RenderTo(
                preview,
                step1GetMorph: (_) =>
                {
                    var morph = new OchalikeMorph();
                    data.AddTo(morph);
                    return morph;
                },
                step2GetOchalikeSprite: (ochalikeSpriteData, bareColor, morph) =>
                {
                    if (ochalikeSpriteData == null) { ochalikeSpriteData = OchalikeSpritePreview.GetOchalikeSpriteData(data); }
                    if (ochalikeSpriteData == null) return OchalikeBone.CreateClearOchalikeSprite(bareColor);
                    else return ochalikeSpriteData.CreateBoneWithHairColor(bareColor, morph);
                });
        }

        public override bool HasPreviewGUI() => targets.Length == 1;
        public override bool RequiresConstantRepaint() => OchalikeSpritePreview.Primary.IsPlaying;
        public override void OnPreviewGUI(Rect r, GUIStyle background) => OchalikeSpritePreview.Primary.OnPreviewGUI(r, preview);
        public override void OnPreviewSettings() => OchalikeSpritePreview.Primary.OnPreviewSettings(enableMorphData: false);
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
            => OchalikeSpritePreview.Primary.RenderStaticPreview(width, height, preview);
    }
}
