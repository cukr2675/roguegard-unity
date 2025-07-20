using OchalikeSprites;
using OchalikeSprites.Editor;
using RoguegardUnity;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Roguegard.CharacterCreation.Editor
{
    /// <summary>
    /// Project ビューにアイコン（<see cref="RogueDescriptionData.Icon"/>）を表示させるエディタ拡張
    /// </summary>
    [CustomEditor(typeof(ScriptableCharacterCreationData), true)]
    public class ScriptableCharacterCreationDataEditor : UnityEditor.Editor
    {
        private RenderTexture preview;
        private RogueObj obj;
        private int motionIndex;

        private void OnEnable()
        {
            preview = RenderTexture.GetTemporary(64, 64, 1);
            preview.autoGenerateMips = false;
            preview.filterMode = FilterMode.Point;
            UpdatePreview();

            OchalikeSpritePreview.Primary.OnUpdatePreview += UpdatePreview;
        }

        private void OnDisable()
        {
            if (preview != null) { RenderTexture.ReleaseTemporary(preview); }

            OchalikeSpritePreview.Primary.OnUpdatePreview -= UpdatePreview;
        }

        private void UpdatePreview()
        {
            if (RoguegardSettings.DefaultRaceOption == null)
            {
                var settingsGuid = AssetDatabase.FindAssets($"t:{nameof(RoguegardSettingsData)}").FirstOrDefault()
                    ?? throw new System.InvalidOperationException($"{nameof(RoguegardSettingsData)} が見つかりません。");

                var settingsPath = AssetDatabase.GUIDToAssetPath(settingsGuid);
                var settings = AssetDatabase.LoadAssetAtPath<RoguegardSettingsData>(settingsPath);

                settings.TestLoad();
                RogueRandom.Primary = new RogueRandom(0);
                MessageWorkListener.ClearListeners();
                StaticId.Next();
            }

            var data = (ScriptableCharacterCreationData)target;
            obj = data.CreateObj(null, Vector2Int.zero, new RogueRandom(0));
            obj.Main.Sprite.Update(obj);

            OchalikeSpritePreview.Primary.RenderTo(
                preview,
                step1GetMorph: delegate { return null; },
                step2GetOchalikeSprite: delegate { return null; },
                step3GetSpriteTransform: (motionData, direction, animationTime) =>
                {
                    var spriteTransform = OchalikeSpriteTransform.Identity;
                    if (motionData != null)
                    {
                        motionData.ApplyTo(animationTime, direction, ref spriteTransform, out _);
                    }
                    else
                    {
                        var motion = motionIndex == 0 ? KeywordSpriteMotion.Wait : KeywordSpriteMotion.Walk;
                        motion.ApplyTo(obj.Main.Sprite.MotionSet, animationTime, direction, ref spriteTransform, out _);
                    }
                    return spriteTransform;
                },
                step4Render: (preview, _, _, spriteTransform, defaultColor) =>
                {
                    var renderController = new OchalikeTextureRenderController
                    {
                        Position = spriteTransform.Position,
                        Rotation = spriteTransform.Rotation,
                        Scale = spriteTransform.Scale
                    };
                    obj.Main.Sprite.SetTo(renderController, spriteTransform.PoseSource.GetSpritePose(spriteTransform.Direction), spriteTransform.Direction);
                    renderController.RenderTo(preview);
                });
        }

        public override bool HasPreviewGUI() => targets.Length == 1;
        public override bool RequiresConstantRepaint() => OchalikeSpritePreview.Primary.IsPlaying;
        public override void OnPreviewGUI(Rect r, GUIStyle background) => OchalikeSpritePreview.Primary.OnPreviewGUI(r, preview);
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
            => OchalikeSpritePreview.Primary.RenderStaticPreview(width, height, preview);

        public override void OnPreviewSettings()
        {
            OchalikeSpritePreview.Primary.OnPreviewSettings(false, false, false);

            EditorGUI.BeginChangeCheck();

            // MotionSet からモーションを選択して再生
            //obj.Main.InfoSet.GetObjSprite(obj, out _, out var motionSet);
            motionIndex = EditorGUILayout.Popup(motionIndex, new[] { "Wait", "Walk" }, GUILayout.Width(96));

            if (EditorGUI.EndChangeCheck()) { UpdatePreview(); }
        }
    }
}
