using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class BlindfoldEquipped : ReferableScript, IEquippedEffectSource
    {
        IEquippedEffect IEquippedEffectSource.CreateOrReuse(RogueObj equipment, IEquippedEffect effect)
        {
            if (effect is Effect effect1 && effect1.Effecter == equipment)
            {
                return effect1;
            }
            else
            {
                return new Effect(equipment);
            }
        }

        private class Effect : BaseEquippedEffect, IStatusEffect, IValueEffect
        {
            string IRogueDescription.Name => "盲目";
            Sprite IRogueDescription.Icon => null;
            Color IRogueDescription.Color => Color.white;
            string IRogueDescription.Caption => null;
            IRogueDetails IRogueDescription.Details => null;
            IKeyword IStatusEffect.EffectCategory => CategoryKw.Equipment;
            IBoneMotion IStatusEffect.HeadIcon => null;
            float IStatusEffect.Order => 0f;

            public RogueObj Effecter { get; }
            float IValueEffect.Order => 0f;

            public Effect(RogueObj equipment)
            {
                Effecter = equipment;
            }

            void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
            {
                if (keyword == StdKw.View)
                {
                    // 視界をゼロにする。
                    value.MainValue = 0f;
                }
            }

            void IStatusEffect.GetEffectedName(RogueNameBuilder refName, RogueObj self) { }
        }
    }
}
