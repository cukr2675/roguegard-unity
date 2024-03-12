using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletalSprite
{
    public class BoneRogueSpriteNode : ISortableBone<BoneRogueSpriteNode>
    {
        public readonly IBone source;

        BoneKeyword ISortableBone<BoneRogueSpriteNode>.Name => source.Name;
        float ISortableBone<BoneRogueSpriteNode>.NormalOrderInParent => source.NormalOrderInParent;
        float ISortableBone<BoneRogueSpriteNode>.BackOrderInParent => source.BackOrderInParent;

        private readonly BoneChildren<BoneRogueSpriteNode> _children;
        ISortableBoneChildren<BoneRogueSpriteNode> ISortableBone<BoneRogueSpriteNode>.Children => _children;

        public BoneBack.Type LocalBack { get; set; }
        public int NormalFrontSpriteCount { get; set; }
        public int NormalRearSpriteCount { get; set; }
        public int BackFrontSpriteCount { get; set; }
        public int BackRearSpriteCount { get; set; }
        public int NormalPoseFrontSpriteIndex { get; set; }
        public int NormalPoseRearSpriteIndex { get; set; }
        public int BackPoseFrontSpriteIndex { get; set; }
        public int BackPoseRearSpriteIndex { get; set; }

        private BoneSprite primarySprite;
        private Color primaryColor;
        private readonly List<BoneSprite> equipmentSprites;
        private readonly List<Color> equipmentColors;
        private bool overridesBaseColor;

        public BoneRogueSpriteNode(IBoneNode boneNode)
        {
            source = boneNode.Bone;
            _children = new BoneChildren<BoneRogueSpriteNode>();
            equipmentSprites = new List<BoneSprite>();
            equipmentColors = new List<Color>();
            for (int i = 0; i < boneNode.Children.Count; i++)
            {
                var childBone = boneNode.Children[i];
                var child = new BoneRogueSpriteNode(childBone);
                _children.AddChild(child);
            }
        }

        public BoneRogueSpriteNode(BoneRogueSpriteNode node)
        {
            source = node.source;
            _children = new BoneChildren<BoneRogueSpriteNode>();
            equipmentSprites = new List<BoneSprite>();
            equipmentColors = new List<Color>();
            for (int i = 0; i < node._children.Count; i++)
            {
                var childBone = node._children[i];
                var child = new BoneRogueSpriteNode(childBone);
                _children.AddChild(child);
            }
        }

        private bool Equals(IBoneNode boneNode)
        {
            if (boneNode.Bone != source) return false;
            if (boneNode.Children.Count != _children.Count) return false;
            for (int i = 0; i < _children.Count; i++)
            {
                var child = _children[i];
                var boneChild = boneNode.Children[i];
                if (!child.Equals(boneChild)) return false;
            }
            return true;
        }

        private void SetBaseSprite(AffectableBoneSpriteTable.RefItem item)
        {
            primarySprite = item.FirstSprite ?? source.Sprite;
            primaryColor = item.OverridesSourceColor ? item.FirstColor : source.Color;
            equipmentSprites.Clear();
            equipmentColors.Clear();
            overridesBaseColor = source.OverridesBaseColor || item.OverridesBaseColor;
            NormalFrontSpriteCount = 0;
            NormalRearSpriteCount = 0;
            BackFrontSpriteCount = 0;
            BackRearSpriteCount = 0;
            if (primarySprite != null)
            {
                if (primarySprite.NormalFront != null) NormalFrontSpriteCount = 1;
                if (primarySprite.NormalRear != null) NormalRearSpriteCount = 1;
                if (primarySprite.BackFront != null) BackFrontSpriteCount = 1;
                if (primarySprite.BackRear != null) BackRearSpriteCount = 1;
            }
        }

        public void ApplyTable(AffectableBoneSpriteTable boneSpriteTable)
        {
            var item = boneSpriteTable.GetSprite(source.Name);
            SetBaseSprite(item);
            for (int i = 0; i < item.EquipmentSpriteCount; i++)
            {
                item.GetEquipmentSprite(i, out var sprite, out var color);
                equipmentSprites.Add(sprite);
                equipmentColors.Add(color);
                if (sprite.NormalFront != null) { NormalFrontSpriteCount++; }
                if (sprite.NormalRear != null) { NormalRearSpriteCount++; }
                if (sprite.BackFront != null) { BackFrontSpriteCount++; }
                if (sprite.BackRear != null) { BackRearSpriteCount++; }
            }
            for (int i = 0; i < _children.Count; i++)
            {
                var child = _children[i];
                child.ApplyTable(boneSpriteTable);
            }
        }

        public void SetTo(
            IRogueObjSpriteRenderController renderController,
            IReadOnlyDictionary<BoneKeyword, BoneTransform> boneTransforms, bool poseBack,
            Vector3 parentPosition, Quaternion parentRotation, Vector3 scaleOfLocalByParent,
            bool parentMirrorX, bool parentMirrorY, Color baseColor)
        {
            // 位置計算
            var position = source.LocalPosition;
            var rotation = source.LocalRotation;
            var scale = source.ScaleOfLocalByLocal;
            var flipX = source.FlipX;
            var flipY = source.FlipY;
            var mirrorX = parentMirrorX;
            var mirrorY = parentMirrorY;
            var back = LocalBack switch
            {
                BoneBack.Type.ForPose => poseBack,
                BoneBack.Type.InversePose => !poseBack,
                BoneBack.Type.ForcedNormal => false,
                BoneBack.Type.ForcedBack => true,
                _ => throw new System.Exception()
            };
            BoneSprite poseSprite = null;
            Color poseColor = default;
            var poseOverridesSourceColor = false;
            if (boneTransforms.TryGetValue(source.Name, out var transform))
            {
                if (transform.TransformsInRootParent)
                {
                    position = Vector3.zero;
                    rotation = Quaternion.identity;
                    scale = Vector3.one;
                }
                position += transform.LocalPosition;
                rotation = transform.LocalRotation * rotation;
                scale = Vector3.Scale(scale, transform.ScaleOfLocalByLocal);
                mirrorX ^= transform.LocalMirrorX;
                mirrorY ^= transform.LocalMirrorY;
                poseSprite = transform.Sprite;
                poseColor = transform.Color;
                poseOverridesSourceColor = transform.OverridesSourceColor;
            }
            if (parentMirrorX) { position.x = -position.x; }
            if (parentMirrorY) { position.y = -position.y; }
            if (mirrorX)
            {
                rotation.x *= -1f;
                rotation.z *= -1f;
                flipX = !flipX;
            }
            if (mirrorY)
            {
                rotation.y *= -1f;
                rotation.z *= -1f;
                flipY = !flipY;
            }
            position = (parentRotation * Vector3.Scale(position, scaleOfLocalByParent)) + parentPosition;
            rotation = parentRotation * rotation;
            scale = Vector3.Scale(scale, scaleOfLocalByParent);

            // スプライト設定
            {
                var frontIndex = 0;
                var rearIndex = 0;
                if (primarySprite != null)
                {
                    if (poseSprite != null || poseOverridesSourceColor)
                    {
                        var boneSprite = poseSprite ?? primarySprite;
                        var color = overridesBaseColor ? primaryColor : baseColor;
                        color = poseOverridesSourceColor ? poseColor : color;
                        SetPoseSprite(boneSprite, color, ref frontIndex, ref rearIndex);
                    }
                    else
                    {
                        var boneSprite = primarySprite;
                        var color = overridesBaseColor ? primaryColor : baseColor;
                        SetSprite(boneSprite, color, ref frontIndex, ref rearIndex);
                    }
                }
                for (int i = 0; i < equipmentSprites.Count; i++)
                {
                    var boneSprite = equipmentSprites[i];
                    var color = equipmentColors[i];
                    SetSprite(boneSprite, color, ref frontIndex, ref rearIndex);
                }
            }

            // 子ボーン更新
            for (int i = 0; i < _children.Count; i++)
            {
                var child = _children[i];
                child.SetTo(renderController, boneTransforms, poseBack, position, rotation, scale, mirrorX, mirrorY, baseColor);
            }

            void SetSprite(BoneSprite boneSprite, Color color, ref int frontIndex, ref int rearIndex)
            {
                var frontSprite = boneSprite.GetFrontSprite(back);
                if (frontSprite != null)
                {
                    var frontBonesIndex = poseBack ? BackPoseFrontSpriteIndex : NormalPoseFrontSpriteIndex;
                    var index = frontBonesIndex + frontIndex;
                    renderController.SetBoneSprite(
                        index, source.Name.Name, frontSprite, color, flipX, flipY, position, rotation, scale);
                    frontIndex++;
                }

                var rearSprite = boneSprite.GetRearSprite(back);
                if (rearSprite != null)
                {
                    var rearBonesIndex = poseBack ? BackPoseRearSpriteIndex : NormalPoseRearSpriteIndex;
                    var index = rearBonesIndex + rearIndex;
                    renderController.SetBoneSprite(
                        index, source.Name.Name, rearSprite, color, flipX, flipY, position, rotation, scale);
                    rearIndex++;
                }
            }

            // ポーズスプライトの上書き前のスプライトが null であった場合は上書きしない。
            void SetPoseSprite(BoneSprite boneSprite, Color color, ref int frontIndex, ref int rearIndex)
            {
                if (primarySprite != null && primarySprite.GetFrontSprite(back) != null)
                {
                    var frontSprite = boneSprite.GetFrontSprite(back);
                    var frontBonesIndex = poseBack ? BackPoseFrontSpriteIndex : NormalPoseFrontSpriteIndex;
                    var index = frontBonesIndex + frontIndex;
                    renderController.SetBoneSprite(
                        index, source.Name.Name, frontSprite, color, flipX, flipY, position, rotation, scale);
                    frontIndex++;
                }
                if (primarySprite != null && primarySprite.GetRearSprite(back) != null)
                {
                    var rearSprite = boneSprite.GetRearSprite(back);
                    var rearBonesIndex = poseBack ? BackPoseRearSpriteIndex : NormalPoseRearSpriteIndex;
                    var index = rearBonesIndex + rearIndex;
                    renderController.SetBoneSprite(
                        index, source.Name.Name, rearSprite, color, flipX, flipY, position, rotation, scale);
                    rearIndex++;
                }
            }
        }
    }
}
