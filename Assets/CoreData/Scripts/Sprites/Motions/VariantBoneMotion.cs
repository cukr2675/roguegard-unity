using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class VariantBoneMotion : IBoneMotion
    {
        private readonly IBoneMotion baseMotion;
        private readonly IKeyword boneName;
        private readonly BoneSprite sprite;
        private readonly bool overridesColor;
        private readonly Color color;
        private readonly Dictionary<IDirectionalBonePoseSource, IDirectionalBonePoseSource> coloredPoseTable;

        public IKeyword Keyword => baseMotion.Keyword;

        public VariantBoneMotion(IBoneMotion baseMotion, Color color)
            : this(baseMotion, null, null, true, color)
        {
        }

        public VariantBoneMotion(IBoneMotion baseMotion, IKeyword boneName, BoneSprite sprite, bool overridesColor, Color color)
        {
            this.baseMotion = baseMotion;
            this.boneName = boneName;
            this.sprite = sprite;
            this.overridesColor = overridesColor;
            this.color = color;
            coloredPoseTable = new Dictionary<IDirectionalBonePoseSource, IDirectionalBonePoseSource>();
        }

        public void ApplyTo(IMotionSet motionSet, int animationTime, RogueDirection direction, ref RogueObjSpriteTransform transform, out bool endOfMotion)
        {
            baseMotion.ApplyTo(motionSet, animationTime, direction, ref transform, out endOfMotion);

            if (!coloredPoseTable.TryGetValue(transform.PoseSource, out var coloredPoseSource))
            {
                coloredPoseSource = new ImmutableVariantBonePoseSource(transform.PoseSource, boneName, sprite, overridesColor, color);
                coloredPoseTable.Add(transform.PoseSource, coloredPoseSource);
            }
            transform.PoseSource = coloredPoseSource;
        }
    }
}
