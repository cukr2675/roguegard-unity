using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    /// <summary>
    /// 指定の <see cref="IDirectionalSpritePoseSource"/> の一部のスプライトと色を変更した <see cref="IDirectionalSpritePoseSource"/> 。
    /// 乗り物にまたがるポーズで乗り物の見た目を反映するときなどに使う。
    /// </summary>
    public class ImmutableVariantSpritePoseSource : IDirectionalSpritePoseSource
    {
        private readonly SpritePose[] poses;

        public ImmutableVariantSpritePoseSource(IDirectionalSpritePoseSource poseSource, IKeyword boneName, BoneSprite sprite, bool overridesColor, Color color)
        {
            poses = new SpritePose[8];
            for (int i = 0; i < 8; i++)
            {
                poses[i] = GetPose(poseSource, new BoneKeyword(boneName.Name), sprite, true, color, new SpriteDirection(i));
            }
        }

        private SpritePose GetPose(
            IDirectionalSpritePoseSource poseSource, BoneKeyword boneName, BoneSprite sprite, bool overridesColor, Color color, SpriteDirection direction)
        {
            var basePose = poseSource.GetSpritePose(direction);
            if (!basePose.IsImmutable) throw new RogueException("元のポーズが不変ではありません。");

            var pose = new SpritePose();
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

        public SpritePose GetSpritePose(SpriteDirection direction)
        {
            return poses[(int)direction];
        }
    }
}
