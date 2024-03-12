using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard.CharacterCreation
{
    public class IllusionBandEquipped : ReferableScript, IEquippedEffectSource
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
            string IRogueDescription.Name => "異性幻像";
            Sprite IRogueDescription.Icon => null;
            Color IRogueDescription.Color => Color.white;
            string IRogueDescription.Caption => "男性は女性 女性は男性と 間違われる状態";
            IRogueDetails IRogueDescription.Details => null;
            IKeyword IStatusEffect.EffectCategory => CategoryKw.Equipment;
            IBoneMotion IStatusEffect.HeadIcon => null;
            float IStatusEffect.Order => 0f;

            public RogueObj Effecter { get; }
            float IValueEffect.Order => 1f;

            public Effect(RogueObj equipment)
            {
                Effecter = equipment;
            }

            void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
            {
                if (keyword == StatsKw.Gender)
                {
                    var selfIsMale = value.SubValues.Is(StatsKw.Male);
                    var selfIsFemale = value.SubValues.Is(StatsKw.Female);
                    if (selfIsMale && (!selfIsFemale))
                    {
                        value.SubValues[StatsKw.LooksMale] = 0f;
                        value.SubValues[StatsKw.LooksFemale] = 1f;
                    }
                    if (selfIsFemale && (!selfIsMale))
                    {
                        value.SubValues[StatsKw.LooksMale] = 1f;
                        value.SubValues[StatsKw.LooksFemale] = 0f;
                    }
                }
            }

            void IStatusEffect.GetEffectedName(RogueNameBuilder refName, RogueObj self) { }
        }
    }
}
