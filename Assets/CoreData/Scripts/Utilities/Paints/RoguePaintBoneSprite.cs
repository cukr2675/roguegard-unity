using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class RoguePaintBoneSprite
    {
        public RoguePaintData NormalFront { get; }
        public RoguePaintData NormalRear { get; }
        public RoguePaintData BackFront { get; }
        public RoguePaintData BackRear { get; }

        public RoguePaintBoneSprite(RoguePaintData normalFront, RoguePaintData normalRear, RoguePaintData backFront, RoguePaintData backRear)
        {
            NormalFront = normalFront;
            NormalRear = normalRear;
            BackFront = backFront;
            BackRear = backRear;
        }

        private RoguePaintBoneSprite() { }

        public BoneSprite ToBoneSprite(Spanning<RoguePaintColor> palette)
        {
            return new BoneSprite(NormalFront.ToSprite(palette), NormalRear.ToSprite(palette), BackFront.ToSprite(palette), BackRear.ToSprite(palette));
        }

        public void ToBoneSprite(
            Spanning<RoguePaintColor> palette, Vector2 upperPivot, Vector2 lowerPivot, out BoneSprite upperBoneSprite, out BoneSprite lowerBoneSprite)
        {
            NormalFront.ToSprite(palette, upperPivot, lowerPivot, out var upperNormalFront, out var lowerNormalFront);
            NormalRear.ToSprite(palette, upperPivot, lowerPivot, out var upperNormalRear, out var lowerNormalRear);
            BackFront.ToSprite(palette, upperPivot, lowerPivot, out var upperBackFront, out var lowerBackFront);
            BackRear.ToSprite(palette, upperPivot, lowerPivot, out var upperBackRear, out var lowerBackRear);
            upperBoneSprite = new BoneSprite(upperNormalFront, upperNormalRear, upperBackFront, upperBackRear);
            lowerBoneSprite = new BoneSprite(lowerNormalFront, lowerNormalRear, lowerBackFront, lowerBackRear);
        }
    }
}
