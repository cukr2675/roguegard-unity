using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    public class OchalikeEvaluatorNode : ISortableBone<OchalikeEvaluatorNode>
    {
        public readonly IReadOnlyOchalikeBone source;

        BoneKeyword ISortableBone<OchalikeEvaluatorNode>.Name => source.Name;
        float ISortableBone<OchalikeEvaluatorNode>.NormalOrderInParent => source.NormalOrderInParent;
        float ISortableBone<OchalikeEvaluatorNode>.BackOrderInParent => source.BackOrderInParent;

        private readonly BoneChildren<OchalikeEvaluatorNode> _children;
        ISortableBoneChildren<OchalikeEvaluatorNode> ISortableBone<OchalikeEvaluatorNode>.Children => _children;

        public BoneBack.Type LocalBack { get; set; }
        public int NormalFrontSpriteCount { get; set; }
        public int NormalRearSpriteCount { get; set; }
        public int BackFrontSpriteCount { get; set; }
        public int BackRearSpriteCount { get; set; }
        public int NormalPoseFrontSpriteIndex { get; set; }
        public int NormalPoseRearSpriteIndex { get; set; }
        public int BackPoseFrontSpriteIndex { get; set; }
        public int BackPoseRearSpriteIndex { get; set; }

        private BoneSprite bareSprite;
        private Color bareColor;
        private readonly List<BoneSprite> equipmentSprites;
        private readonly List<Color> equipmentColors;
        private bool overridesOnDefaultColor;

        public OchalikeEvaluatorNode(IReadOnlyOchalikeBone bone)
        {
            source = bone;
            _children = new BoneChildren<OchalikeEvaluatorNode>();
            equipmentSprites = new List<BoneSprite>();
            equipmentColors = new List<Color>();
            for (int i = 0; i < bone.Children.Count; i++)
            {
                var childBone = bone.Children[i];
                var child = new OchalikeEvaluatorNode(childBone);
                _children.AddChild(child);
            }
        }

        public OchalikeEvaluatorNode(OchalikeEvaluatorNode node)
        {
            source = node.source;
            _children = new BoneChildren<OchalikeEvaluatorNode>();
            equipmentSprites = new List<BoneSprite>();
            equipmentColors = new List<Color>();
            for (int i = 0; i < node._children.Count; i++)
            {
                var childBone = node._children[i];
                var child = new OchalikeEvaluatorNode(childBone);
                _children.AddChild(child);
            }
        }

        private bool Equals(IReadOnlyOchalikeBone bone)
        {
            if (bone != source) return false;
            if (bone.Children.Count != _children.Count) return false;
            for (int i = 0; i < _children.Count; i++)
            {
                var child = _children[i];
                var boneChild = bone.Children[i];
                if (!child.Equals(boneChild)) return false;
            }
            return true;
        }

        private void SetBareSprite(OchalikeMorph.RefItem morphItem)
        {
            bareSprite = morphItem.MorphBareSprite ?? source.BareSprite;
            bareColor = morphItem.MorphBareColor ?? source.BareColor;
            equipmentSprites.Clear();
            equipmentColors.Clear();
            overridesOnDefaultColor = source.OverridesOnDefaultColor || morphItem.OverridesOnDefaultColor;
            NormalFrontSpriteCount = 0;
            NormalRearSpriteCount = 0;
            BackFrontSpriteCount = 0;
            BackRearSpriteCount = 0;
            if (bareSprite != null)
            {
                if (bareSprite.NormalFront != null) NormalFrontSpriteCount = 1;
                if (bareSprite.NormalRear != null) NormalRearSpriteCount = 1;
                if (bareSprite.BackFront != null) BackFrontSpriteCount = 1;
                if (bareSprite.BackRear != null) BackRearSpriteCount = 1;
            }
        }

        public void ApplyTable(OchalikeMorph ochalikeMorph)
        {
            var item = ochalikeMorph.GetSprite(source.Name);
            SetBareSprite(item);
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
                child.ApplyTable(ochalikeMorph);
            }
        }

        public void SetTo(
            IOchalikeSpriteRenderController renderController,
            IReadOnlyDictionary<BoneKeyword, SpritePoseBoneTransform> boneTransforms, bool poseBack,
            Vector3 parentPosition, Quaternion parentRotation, Vector3 scaleOfLocalByParent,
            bool parentMirrorX, bool parentMirrorY, Color defaultColor)
        {
            // 位置計算
            var position = source.LocalPosition;
            var rotation = source.LocalRotation;
            var scale = source.ScaleOfLocalByLocal;
            var flipX = source.FlipX;
            var flipY = source.FlipY;
            var mirrorX = parentMirrorX;
            var mirrorY = parentMirrorY;
            var isBack = LocalBack switch
            {
                BoneBack.Type.ForPose => poseBack,
                BoneBack.Type.InversePose => !poseBack,
                BoneBack.Type.ForcedNormal => false,
                BoneBack.Type.ForcedBack => true,
                _ => throw new System.Exception()
            };
            BoneSprite poseBareSprite = null;
            Color? poseBareColor = default;
            var transformsInRootParent = false;
            if (boneTransforms.TryGetValue(source.Name, out var transform))
            {
                if (transform.TransformsInRootParent)
                {
                    position = Vector3.zero;
                    rotation = Quaternion.identity;
                    scale = Vector3.one;
                    transformsInRootParent = true;
                }
                position += transform.LocalPosition;
                rotation = transform.LocalRotation * rotation;
                scale = Vector3.Scale(scale, transform.ScaleOfLocalByLocal);
                mirrorX ^= transform.LocalMirrorX;
                mirrorY ^= transform.LocalMirrorY;
                poseBareSprite = transform.PoseBareSprite;
                poseBareColor = transform.PoseBareColor;
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
            if (!transformsInRootParent)
            {
                position = (parentRotation * Vector3.Scale(position, scaleOfLocalByParent)) + parentPosition;
                rotation = parentRotation * rotation;
                scale = Vector3.Scale(scale, scaleOfLocalByParent);
            }

            // スプライト設定
            {
                var frontIndex = 0;
                var rearIndex = 0;
                if (bareSprite != null)
                {
                    var boneSprite = bareSprite;
                    var color = overridesOnDefaultColor ? bareColor : defaultColor;
                    if (poseBareSprite != null || poseBareColor != null)
                    {
                        boneSprite = poseBareSprite ?? boneSprite;
                        color = poseBareColor ?? color;
                        SetPoseSprite(boneSprite, color, ref frontIndex, ref rearIndex);
                    }
                    else
                    {
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
                child.SetTo(renderController, boneTransforms, poseBack, position, rotation, scale, mirrorX, mirrorY, defaultColor);
            }

            void SetSprite(BoneSprite boneSprite, Color color, ref int frontIndex, ref int rearIndex)
            {
                var frontSprite = boneSprite.GetFrontSprite(isBack);
                if (frontSprite != null)
                {
                    var frontBonesIndex = poseBack ? BackPoseFrontSpriteIndex : NormalPoseFrontSpriteIndex;
                    var index = frontBonesIndex + frontIndex;
                    renderController.SetBoneSprite(
                        index, source.Name.Name, frontSprite, color, flipX, flipY, position, rotation, scale);
                    frontIndex++;
                }

                var rearSprite = boneSprite.GetRearSprite(isBack);
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
                if (bareSprite != null && bareSprite.GetFrontSprite(isBack) != null)
                {
                    var frontSprite = boneSprite.GetFrontSprite(isBack);
                    var frontBonesIndex = poseBack ? BackPoseFrontSpriteIndex : NormalPoseFrontSpriteIndex;
                    var index = frontBonesIndex + frontIndex;
                    renderController.SetBoneSprite(
                        index, source.Name.Name, frontSprite, color, flipX, flipY, position, rotation, scale);
                    frontIndex++;
                }
                if (bareSprite != null && bareSprite.GetRearSprite(isBack) != null)
                {
                    var rearSprite = boneSprite.GetRearSprite(isBack);
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
