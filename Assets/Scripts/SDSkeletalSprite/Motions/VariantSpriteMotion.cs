using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDSSprite
{
    public class VariantSpriteMotion : ISpriteMotion
    {
        private readonly ISpriteMotion baseMotion;
        private readonly BoneKeyword boneName;
        private readonly BoneSprite sprite;
        private readonly bool overridesColor;
        private readonly Color color;
        private readonly Dictionary<IDirectionalSpritePoseSource, IDirectionalSpritePoseSource> coloredPoseTable;

        public VariantSpriteMotion(ISpriteMotion baseMotion, Color color)
            : this(baseMotion, BoneKeyword.Other, null, true, color)
        {
        }

        public VariantSpriteMotion(ISpriteMotion baseMotion, BoneKeyword boneName, BoneSprite sprite, bool overridesColor, Color color)
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
                coloredPoseSource = new ImmutableVariantSpritePoseSource(transform.PoseSource, boneName, sprite, overridesColor, color);
                coloredPoseTable.Add(transform.PoseSource, coloredPoseSource);
            }
            transform.PoseSource = coloredPoseSource;
        }
    }
}
