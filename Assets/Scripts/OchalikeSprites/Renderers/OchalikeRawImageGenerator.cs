using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OchalikeSprites
{
    public class OchalikeRawImageGenerator : MonoBehaviour
    {
        [Header("Ochalike Sprites")]
        [SerializeField] private OchalikeSpriteAsset _ochalikeSprite = null;
        [SerializeField] private Color _defaultColor = Color.white;
        [SerializeField] private Color _bareColor = Color.white;
        [SerializeField] private bool _useDarkOutline = false;
        [SerializeField] private OchalikeMorphAsset _morph = null;
        [SerializeField] private SpriteMotionAsset _motion = null;
        [SerializeField] private int _angle = 6;
        [SerializeField] private Vector2Int _spriteSize = new(64, 64);
        [SerializeField] private Vector2 _spriteOffset = new(0f, -.5f);
        [SerializeField] private float _pixelsPerUnit = OchalikeSpritesUtility.DefaultPixelsPerUnit;
        [SerializeField] private Material _material = null;

        private RawImage rawImage;
        private RenderTexture renderTexture;
        private RenderController renderController;

        private OchalikeEvaluatorNode root;
        private ISpriteMotion motion;
        private int motionTime;
        private BoneOrder enabledOrder;
        private int normalBonesCount;
        private int backBonesCount;
        private SpritePose enabledImmutablePose;

        protected virtual void Awake()
        {
            if (renderController != null) return;

            var rawImageObj = new GameObject($"{name} (RawImage)");
            var rawImageTransform = rawImageObj.AddComponent<RectTransform>();
            rawImageTransform.SetParent(transform, false);
            rawImageTransform.anchorMin = Vector2.zero;
            rawImageTransform.anchorMax = Vector2.one;
            rawImageTransform.sizeDelta = Vector2.zero;
            rawImageTransform.pivot = ((RectTransform)transform).pivot;
            rawImageTransform.anchoredPosition = Vector2.zero;
            rawImage = rawImageObj.AddComponent<RawImage>();
            rawImage.raycastTarget = false;

            if (renderTexture != null) { RenderTexture.ReleaseTemporary(renderTexture); }
            renderTexture = RenderTexture.GetTemporary(_spriteSize.x, _spriteSize.y, 1);
            renderTexture.autoGenerateMips = false;
            renderTexture.filterMode = FilterMode.Point;
            rawImage.texture = renderTexture;

            renderController = new RenderController() { material = _material, offset = _spriteOffset };
        }

        protected virtual void OnDestroy()
        {
            RenderTexture.ReleaseTemporary(renderTexture);
        }

        public void SetSprite(OchalikeBone ochalikeSprite = null, OchalikeMorph morph = null, ISpriteMotion motion = null)
        {
            Awake();

            if (ochalikeSprite != null)
            {
                root = new OchalikeEvaluatorNode(ochalikeSprite);
            }
            else if (root == null && _ochalikeSprite != null)
            {
                ochalikeSprite = _ochalikeSprite.CreateOchalikeSprite(_bareColor, _useDarkOutline);
                root = new OchalikeEvaluatorNode(ochalikeSprite);
                var rootMorph = new OchalikeMorph();
                _morph.AddTo(rootMorph);
                root.ApplyMorph(rootMorph);
            }

            if (morph != null)
            {
                root.ApplyMorph(morph);
            }

            if (motion != null)
            {
                this.motion = motion;
            }
            else if (this.motion == null)
            {
                this.motion = _motion;
            }

            motionTime = 0;
        }

        private void UpdateIndex(BoneOrder boneOrder)
        {
            // 引数のオーダーが前回のオーダーと同じであれば、再ソートする必要はない。
            if (BoneOrder.Equals(enabledOrder, boneOrder)) return;

            normalBonesCount = BoneSorter.SetIndexAndGetCount(root, boneOrder, false);
            backBonesCount = BoneSorter.SetIndexAndGetCount(root, boneOrder, true);
            enabledOrder = boneOrder;
        }

        private void SetTo(SpritePose pose)
        {
            // 引数のポーズが前回のポーズと同じかつ不変であれば、更新する必要はない。
            if (enabledOrder != null && enabledImmutablePose == pose) return;

            UpdateIndex(pose.BoneOrder);

            var bonesCount = pose.Back ? backBonesCount : normalBonesCount;
            renderController.AdjustBones(bonesCount);
            renderController.ClearBoneSprites();

            root.SetTo(
                renderController, pose.BoneTransforms, pose.Back, Vector3.zero, Quaternion.identity, Vector3.one, false, false,
                _defaultColor);

            renderController.RenderTo(renderTexture);

            if (pose.IsImmutable) enabledImmutablePose = pose;
            else enabledImmutablePose = null;
        }

        protected virtual void LateUpdate()
        {
            if (root == null) return;

            var spriteTransform = OchalikeSpriteTransform.Identity;
            var direction = new SpriteDirection(_angle);
            motion.ApplyTo(motionTime, direction, ref spriteTransform, out _);
            var pose = spriteTransform.PoseSource.GetSpritePose(spriteTransform.Direction);
            SetTo(pose);
            var rawImageTransform = (RectTransform)rawImage.transform;
            rawImageTransform.localPosition = spriteTransform.Position * rawImageTransform.rect.size * _pixelsPerUnit / _spriteSize;
            rawImageTransform.localRotation = spriteTransform.Rotation;
            rawImageTransform.localScale = spriteTransform.Scale;

            motionTime += 1;
        }

        [ContextMenu("Preview")]
        private void Preview()
        {
            Awake();
            LateUpdate();
        }

        private class RenderController : IOchalikeSpriteRenderController
        {
            public Vector3 offset;
            public Material material;
            private readonly List<Bone> bones = new();

            public int Count => bones.Count;

            public void AdjustBones(int bonesCount)
            {
                while (bones.Count < bonesCount)
                {
                    bones.Add(new Bone() { material = material });
                }
                if (bones.Count > bonesCount)
                {
                    bones.RemoveRange(bonesCount, bones.Count - bonesCount);
                }
            }

            public void ClearBoneSprites()
            {
                foreach (var bone in bones)
                {
                    bone.Clear();
                }
            }

            public void SetBoneSprite(
                int index, string name, Sprite sprite, Color color, bool flipX, bool flipY, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
            {
                bones[index].SetSprite(name, sprite, color, flipX, flipY, localPosition + offset, localRotation, localScale);
            }

            public void RenderTo(RenderTexture renderTexture)
            {
                RenderTexture.active = renderTexture;
                GL.Clear(true, true, Color.clear);
                foreach (var bone in bones)
                {
                    bone.RenderTo(renderTexture);
                }
                RenderTexture.active = null;
            }
        }

        private class Bone
        {
            public Material material;
            private string name;
            private Sprite sprite;
            private Color color;
            private bool flipX, flipY;
            private Vector3 localPosition;
            private Quaternion localRotation;
            private Vector3 localScale;

            public void Clear()
            {
                sprite = null;
            }

            public void SetSprite(
                string name, Sprite sprite, Color color, bool flipX, bool flipY,
                Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
            {
#if UNITY_EDITOR
                if (this.sprite != null) { Debug.LogWarning($"スプライトの上書きが発生しました。 ({this.name} -> {name})"); }
#endif
                this.name = name;
                this.sprite = sprite;
                this.color = color;
                this.flipX = flipX;
                this.flipY = flipY;
                this.localPosition = localPosition;
                this.localRotation = localRotation;
                this.localScale = localScale;
            }

            public void RenderTo(RenderTexture renderTexture)
            {
                if (sprite == null) return;

                var spriteRect = sprite.rect;
                var spriteTextureSize = new Vector2(sprite.texture.width, sprite.texture.height);
                var renderTextureSize = new Vector2(renderTexture.width, renderTexture.height);

                var sourceRect = spriteRect;
                sourceRect.position /= spriteTextureSize;
                sourceRect.size /= spriteTextureSize;

                var fx = flipX ? -1f : +1f;
                var fy = flipY ? -1f : +1f;

                GL.PushMatrix();
                GL.modelview = Matrix4x4.identity; // カメラ回転を影響を受けないようにする　これがないと歪むことがある
                GL.LoadProjectionMatrix(
                    Matrix4x4.TRS(localPosition, localRotation, localScale) *                               // ④ 変形を適用
                    Matrix4x4.Scale(new Vector3(fx, -fy, 1f) / sprite.pixelsPerUnit) *                      // ③ スプライトをユニット補正 & フリップを適用
                    Matrix4x4.Translate(-new Vector2(sprite.pivot.x, spriteRect.height - sprite.pivot.y)) * // ② スプライトの pivot を中心に移動
                    Matrix4x4.Scale(spriteRect.size / renderTextureSize)                                    // ① RenderTexture から spriteRect へ変換
                    );

                Graphics.DrawTexture(new Rect(Vector2.zero, renderTextureSize), sprite.texture, sourceRect, 0, 0, 0, 0, color, material);
                GL.PopMatrix();
            }
        }
    }
}
