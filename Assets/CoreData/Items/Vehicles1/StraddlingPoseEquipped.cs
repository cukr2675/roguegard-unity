using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class StraddlingPoseEquipped : ReferableScript, IEquippedEffectSource
    {
        [SerializeField] private DirectionalBonePoseSourceData _pose;
        [SerializeField] private KeywordData _vehicleBoneName;
        [SerializeField] private BoneSprite _vehicleBoneSprite;

        IEquippedEffect IEquippedEffectSource.CreateOrReuse(RogueObj equipment, IEquippedEffect effect)
        {
            if (effect is Effect effect1)
            {
                effect1.PoseSource = null;
                return effect1;
            }
            else
            {
                return new Effect(this, equipment);
            }
        }

        private class Effect : BaseEquippedEffect, IBoneMotionEffect
        {
            private readonly StraddlingPoseEquipped parent;
            private readonly RogueObj equipment;

            public ImmutableVariantBonePoseSource PoseSource { get; set; }

            float IBoneMotionEffect.Order => -1f;

            public Effect(StraddlingPoseEquipped parent, RogueObj equipment)
            {
                this.parent = parent;
                this.equipment = equipment;
            }

            void IBoneMotionEffect.ApplyTo(
                IMotionSet motionSet, IKeyword keyword, int animationTime, RogueDirection direction, ref RogueObjSpriteTransform transform)
            {
                if (keyword == MainInfoKw.Wait || keyword == MainInfoKw.Walk)
                {
                    if (PoseSource == null)
                    {
                        var color = RogueColorUtility.GetColor(equipment);
                        PoseSource = new ImmutableVariantBonePoseSource(parent._pose, parent._vehicleBoneName, parent._vehicleBoneSprite, true, color);
                    }

                    transform.PoseSource = PoseSource;
                }
            }
        }
    }
}
