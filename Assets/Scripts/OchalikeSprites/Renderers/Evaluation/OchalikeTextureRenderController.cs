using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    public class OchalikeTextureRenderController : IOchalikeSpriteRenderController
    {
        private readonly List<Bone> bones = new();

        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 PositionOffset { get; set; } = DefaultPositionOffset;
        public Vector3 ScaleOffset { get; set; } = DefaultScaleOffset;
        public Material Material { get; set; } = new Material(Shader.Find("Ochalike Sprites/Sprites/Custom Shift"));
        public int PixelsPerUnit { get; set; } = OchalikeSpritesUtility.DefaultPixelsPerUnit;

        public int Count => bones.Count;

        public static Vector3 DefaultPositionOffset => new(0f, -0.75f);
        public static Vector3 DefaultScaleOffset => Vector3.one;

        public void Set(IReadOnlyOchalikeBone ochalikeSprite, OchalikeMorph morph, OchalikeSpriteTransform spriteTransform, Color defaultColor)
        {
            // 評価ノードを初期化
            var root = new OchalikeEvaluatorNode(ochalikeSprite); // OchalikeSprite を読み込み
            root.ApplyTable(morph); // 見た目を適用
            var pose = spriteTransform.PoseSource.GetSpritePose(spriteTransform.Direction); // ポーズを取得
            var bonesCount = BoneSorter.SetIndexAndGetCount(root, pose.BoneOrder, pose.Back); // ポーズを適用

            // RenderController に SpriteTransform を設定する
            Position = spriteTransform.Position;
            Rotation = spriteTransform.Rotation;
            Scale = spriteTransform.Scale;

            // RenderController に OchalikeSprite を設定する
            AdjustBones(bonesCount);
            ClearBoneSprites();
            root.SetTo(this, pose.BoneTransforms, pose.Back, Vector3.zero, Quaternion.identity, Vector3.one, false, false, defaultColor);
        }

        public void AdjustBones(int bonesCount)
        {
            while (bones.Count < bonesCount)
            {
                bones.Add(new Bone());
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
            bones[index].SetSprite(name, sprite, color, flipX, flipY, localPosition, localRotation, localScale);
        }

        public void RenderTo(RenderTexture renderTexture)
        {
            var position = Position;
            position.x = Mathf.Round(Position.x * PixelsPerUnit) / PixelsPerUnit;
            position.y = Mathf.Round(Position.y * PixelsPerUnit) / PixelsPerUnit;

            RenderTexture.active = renderTexture;
            GL.PushMatrix();
            try
            {
                GL.modelview = Matrix4x4.identity; // カメラ回転を影響を受けないようにする　これがないと歪むことがある
                GL.Clear(true, true, Color.clear);
                foreach (var bone in bones)
                {
                    bone.GLDraw(renderTexture, Vector3.Scale(position, ScaleOffset) + PositionOffset, Rotation, Vector3.Scale(Scale, ScaleOffset), Material);
                }
            }
            finally
            {
                RenderTexture.active = null;
                GL.PopMatrix();
            }
        }

        private class Bone
        {
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

            public void GLDraw(RenderTexture renderTexture, Vector3 position, Quaternion rotation, Vector3 scale, Material material)
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

                GL.LoadProjectionMatrix(
                    Matrix4x4.TRS(position, rotation, scale) *                                              // ⑤ 全体の変形を適用
                    Matrix4x4.TRS(localPosition, localRotation, localScale) *                               // ④ ボーンの変形を適用
                    Matrix4x4.Scale(new Vector3(fx, -fy, 1f) / sprite.pixelsPerUnit) *                      // ③ スプライトをユニット補正 & フリップを適用
                    Matrix4x4.Translate(-new Vector2(sprite.pivot.x, spriteRect.height - sprite.pivot.y)) * // ② スプライトの pivot を中心に移動
                    Matrix4x4.Scale(spriteRect.size / renderTextureSize)                                    // ① RenderTexture から spriteRect へ変換
                    );

                Graphics.DrawTexture(new Rect(Vector2.zero, renderTextureSize), sprite.texture, sourceRect, 0, 0, 0, 0, color, material);
            }
        }
    }
}
