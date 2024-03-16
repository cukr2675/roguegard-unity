using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    public class VariantBoneMotion : ISpriteMotion
    {
        private readonly ISpriteMotion baseMotion;
        private readonly IKeyword boneName;
        private readonly BoneSprite sprite;
        private readonly bool overridesColor;
        private readonly Color color;
        private readonly Dictionary<IDirectionalSpritePoseSource, IDirectionalSpritePoseSource> coloredPoseTable;

        public VariantBoneMotion(ISpriteMotion baseMotion, Color color)
            : this(baseMotion, null, null, true, color)
        {
        }

        public VariantBoneMotion(ISpriteMotion baseMotion, IKeyword boneName, BoneSprite sprite, bool overridesColor, Color color)
        {
            this.baseMotion = baseMotion;
            this.boneName = boneName;
            this.sprite = sprite;
            this.overridesColor = overridesColor;
            this.color = color;
            coloredPoseTable = new Dictionary<IDirectionalSpritePoseSource, IDirectionalSpritePoseSource>();
        }

        public void ApplyTo(int animationTime, SpriteDirection direction, ref SkeletalSpriteTransform transform, out bool endOfMotion)
        {
            baseMotion.ApplyTo(animationTime, direction, ref transform, out endOfMotion);

            if (!coloredPoseTable.TryGetValue(transform.PoseSource, out var coloredPoseSource))
            {
                coloredPoseSource = new ImmutableVariantBonePoseSource(transform.PoseSource, boneName, sprite, overridesColor, color);
                coloredPoseTable.Add(transform.PoseSource, coloredPoseSource);
            }
            transform.PoseSource = coloredPoseSource;
        }
    }
}
