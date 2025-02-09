using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    public class VariantSpriteMotion : ISpriteMotion
    {
        private readonly ISpriteMotion baseMotion;
        private readonly BoneKeyword variantTargetBoneName;
        private readonly BoneSprite poseBareSprite;
        private readonly Color? poseBareColor;
        private readonly Dictionary<IDirectionalSpritePoseSource, IDirectionalSpritePoseSource> coloredPoseTable;

        public VariantSpriteMotion(ISpriteMotion baseMotion, Color poseBareColor)
            : this(baseMotion, BoneKeyword.Free, null, poseBareColor)
        {
        }

        public VariantSpriteMotion(ISpriteMotion baseMotion, BoneKeyword variantTargetBoneName, BoneSprite poseBareSprite, Color? poseBareColor)
        {
            this.baseMotion = baseMotion;
            this.variantTargetBoneName = variantTargetBoneName;
            this.poseBareSprite = poseBareSprite;
            this.poseBareColor = poseBareColor;
            coloredPoseTable = new Dictionary<IDirectionalSpritePoseSource, IDirectionalSpritePoseSource>();
        }

        public void ApplyTo(int animationTime, SpriteDirection direction, ref OchalikeSpriteTransform transform, out bool endOfMotion)
        {
            baseMotion.ApplyTo(animationTime, direction, ref transform, out endOfMotion);

            if (!coloredPoseTable.TryGetValue(transform.PoseSource, out var coloredPoseSource))
            {
                coloredPoseSource = new ImmutableVariantSpritePoseSource(transform.PoseSource, variantTargetBoneName, poseBareSprite, poseBareColor);
                coloredPoseTable.Add(transform.PoseSource, coloredPoseSource);
            }
            transform.PoseSource = coloredPoseSource;
        }
    }
}
