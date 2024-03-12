using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    /// <summary>
    /// 指定の <see cref="IDirectionalBonePoseSource"/> の一部のスプライトと色を変更した <see cref="IDirectionalBonePoseSource"/> 。
    /// 乗り物にまたがるポーズで乗り物の見た目を反映するときなどに使う。
    /// </summary>
    public class ImmutableVariantBonePoseSource : IDirectionalBonePoseSource
    {
        private readonly BonePose[] poses;

        public ImmutableVariantBonePoseSource(IDirectionalBonePoseSource poseSource, IKeyword boneName, BoneSprite sprite, bool overridesColor, Color color)
        {
            poses = new BonePose[8];
            for (int i = 0; i < 8; i++)
            {
                poses[i] = GetPose(poseSource, new BoneKeyword(boneName.Name), sprite, true, color, new SpriteDirection(i));
            }
        }

        private BonePose GetPose(
            IDirectionalBonePoseSource poseSource, BoneKeyword boneName, BoneSprite sprite, bool overridesColor, Color color, SpriteDirection direction)
        {
            var basePose = poseSource.GetBonePose(direction);
            if (!basePose.IsImmutable) throw new RogueException("元のポーズが不変ではありません。");

            var pose = new BonePose();
            foreach (var pair in basePose.BoneTransforms)
            {
                var transform = pair.Value;
                if (boneName == null || pair.Key == boneName)
                {
                    var overridesTransformColor = overridesColor && transform.OverridesSourceColor;
                    transform = new BoneTransform(
                        sprite ?? transform.Sprite, overridesTransformColor ? color : transform.Color,
                        transform.OverridesSourceColor, transform.LocalPosition, transform.LocalRotation, transform.ScaleOfLocalByLocal,
                        transform.TransformsInRootParent, transform.LocalMirrorX, transform.LocalMirrorY);
                }

                pose.AddBoneTransform(transform, pair.Key);
            }
            pose.SetBack(basePose.Back);
            pose.SetBoneOrder(basePose.BoneOrder);
            pose.SetImmutable();
            return pose;
        }

        public BonePose GetBonePose(SpriteDirection direction)
        {
            return poses[(int)direction];
        }
    }
}
