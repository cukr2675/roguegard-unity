using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace OchalikeSprites.Editor
{
    /// <summary>
    /// <see cref="UnityEditor.Editor"/> で OchalikeSprite のプレビューを表示するクラス
    /// </summary>
    public class OchalikeSpritePreview
    {
        public OchalikeSpriteData OchalikeSpriteData { get; set; } = null;
        public Color BareColor { get; set; } = Color.white;
        public OchalikeMorphData MorphData { get; set; } = null;
        public SpriteMotionData MotionData { get; set; } = null;
        public SpriteDirection Direction { get; set; } = SpriteDirection.LowerLeft;
        public Color DefaultColor { get; set; } = Color.white;

        public bool IsPlaying { get; private set; } = false;
        private double beforeTimeSinceStartup = 0.0;
        private float animationTime = 0f;

        /// <summary>
        /// <see cref="OnPreviewGUI"/> と <see cref="OnPreviewSettings"/> で変更があったとき呼び出されるイベント
        /// </summary>
        public event System.Action OnUpdatePreview;

        public static OchalikeSpritePreview Primary { get; set; } = new();

        public delegate OchalikeMorph GetMorph(OchalikeMorphData morphData);
        public delegate IReadOnlyOchalikeBone GetOchalikeSprite(OchalikeSpriteData ochalikeSpriteData, Color bareColor, OchalikeMorph morph);
        public delegate OchalikeSpriteTransform GetSpriteTransform(SpriteMotionData motionData, SpriteDirection direction, int animationTime);
        public delegate void Render(
            RenderTexture preview, IReadOnlyOchalikeBone ochalikeSprite, OchalikeMorph morph, OchalikeSpriteTransform spriteTransform, Color defaultColor);

        /// <summary>
        /// プレビュー情報生成ステップごとに処理を変更してレンダリングする
        /// </summary>
        /// <param name="preview">レンダリング対象</param>
        /// <param name="step1GetMorph"><see cref="OchalikeMorph"/> 生成ステップ</param>
        /// <param name="step2GetOchalikeSprite">OchalikeSprite 生成ステップ</param>
        /// <param name="step3GetSpriteTransform"><see cref="OchalikeSpriteTransform"/> 生成ステップ</param>
        /// <param name="step4Render">レンダリングステップ</param>
        public void RenderTo(
            RenderTexture preview, GetMorph step1GetMorph = null, GetOchalikeSprite step2GetOchalikeSprite = null,
            GetSpriteTransform step3GetSpriteTransform = null, Render step4Render = null)
        {
            // null ならば初期値を使用
            step1GetMorph ??= (morphData) =>
            {
                var morph = new OchalikeMorph();
                if (morphData != null) { morphData.AddTo(morph); }
                return morph;
            };
            step2GetOchalikeSprite ??= (ochalikeSpriteData, bareColor, morph) =>
            {
                if (ochalikeSpriteData != null) return ochalikeSpriteData.CreateBoneWithHairColor(bareColor, morph);
                else return OchalikeBone.CreateClearOchalikeSprite(bareColor);
            };
            step3GetSpriteTransform ??= (motionData, direction, animationTime) =>
            {
                var spriteTransform = OchalikeSpriteTransform.Identity;
                if (motionData != null) { motionData.ApplyTo(animationTime, direction, ref spriteTransform, out _); }
                else { spriteTransform.Direction = direction; }
                return spriteTransform;
            };
            step4Render ??= (preview, ochalikeSprite, morph, spriteTransform, defaultColor) =>
            {
                var renderController = new OchalikeTextureRenderController();
                renderController.Set(ochalikeSprite, morph, spriteTransform, defaultColor);
                renderController.RenderTo(preview);
            };

            // プレビュー情報生成
            var morph = step1GetMorph(MorphData);
            var ochalikeSprite = step2GetOchalikeSprite(OchalikeSpriteData, BareColor, morph);
            var spriteTransform = step3GetSpriteTransform(MotionData, Direction, Mathf.FloorToInt(animationTime));

            // レンダリング
            step4Render(preview, ochalikeSprite, morph, spriteTransform, DefaultColor);
        }

        public void OnPreviewGUI(Rect r, Texture preview)
        {
            var titleHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2;
            var playButtonRect = new Rect(0f, titleHeight, 64f, EditorGUIUtility.singleLineHeight);
            var seekBarRect = new Rect(playButtonRect.width, titleHeight, r.width - playButtonRect.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginChangeCheck();

            if (GUI.Button(playButtonRect, !IsPlaying ? "Play" : "Pause"))
            {
                if (!IsPlaying)
                {
                    // 再生
                    IsPlaying = true;
                    beforeTimeSinceStartup = EditorApplication.timeSinceStartup;
                    EditorApplication.update += UpdatePlayback;
                }
                else
                {
                    // 停止
                    IsPlaying = false;
                    EditorApplication.update -= UpdatePlayback;
                }
            }
            animationTime = GUI.HorizontalSlider(seekBarRect, animationTime, 0f, 100f);

            if (EditorGUI.EndChangeCheck()) { OnUpdatePreview?.Invoke(); }

            GUI.DrawTexture(r, preview, ScaleMode.ScaleToFit);
        }

        private void UpdatePlayback()
        {
            var deltaTime = (float)(EditorApplication.timeSinceStartup - beforeTimeSinceStartup);
            beforeTimeSinceStartup = EditorApplication.timeSinceStartup;

            animationTime = Mathf.Repeat(animationTime + deltaTime * 60f, 100f);
            OnUpdatePreview?.Invoke();
        }

        /// <summary>
        /// このインスタンスの各プロパティの入力欄を表示する。引数で表示/非表示を切り替える。
        /// </summary>
        public void OnPreviewSettings(
            bool enableOchalikeSpriteData = true, bool enableBareColor = true, bool enableMorphData = true,
            bool enableMotionData = true, bool enableDirection = true)
        {
            EditorGUI.BeginChangeCheck();

            if (enableOchalikeSpriteData)
            {
                OchalikeSpriteData = (OchalikeSpriteData)EditorGUILayout.ObjectField(OchalikeSpriteData, typeof(OchalikeSpriteData), false, GUILayout.Width(96));
            }
            if (enableBareColor)
            {
                BareColor = EditorGUILayout.ColorField(BareColor, GUILayout.Width(48));
            }
            if (enableMorphData)
            {
                MorphData = (OchalikeMorphData)EditorGUILayout.ObjectField(MorphData, typeof(OchalikeMorphData), false, GUILayout.Width(96));
            }
            if (enableMotionData)
            {
                MotionData = (SpriteMotionData)EditorGUILayout.ObjectField(MotionData, typeof(SpriteMotionData), false, GUILayout.Width(96));
            }
            if (enableDirection)
            {
                var angle = EditorGUILayout.Popup((int)Direction, new[] { "→", "↗", "↑", "↖", "←", "↙", "↓", "↘" }, GUILayout.Width(32));
                Direction = new SpriteDirection(angle);
            }

            if (EditorGUI.EndChangeCheck()) { OnUpdatePreview?.Invoke(); }
        }

        public Texture2D RenderStaticPreview(int width, int height, Texture preview)
        {
            var scaledPreview = new Texture2D(width, height, TextureFormat.RGBA32, false);
            OchalikeTexture.ScalingCopy(preview, scaledPreview);
            scaledPreview.Apply();
            return scaledPreview;
        }

        /// <summary>
        /// リフレクションを使用してプレビュー用の <see cref="OchalikeSpriteData"/> を取得する
        /// </summary>
        internal static OchalikeSpriteData GetOchalikeSpriteData(Object target)
        {
            var propertyInfo = target.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) // public, private を問わないインスタンスフィールドで
                .Where(p => p.FieldType == typeof(OchalikeSpriteData))                           // OchalikeSpriteData 型のフィールドを取得
                .FirstOrDefault();
            if (propertyInfo == null) return null; // 該当するフィールドが見つからなければ null

            return (OchalikeSpriteData)propertyInfo.GetValue(target);
        }
    }
}
