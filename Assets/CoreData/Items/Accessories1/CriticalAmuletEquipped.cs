using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class CriticalAmuletEquipped : ReferableScript, IEquippedEffectSource
    {
        private static readonly Effect effect = new Effect();

        IEquippedEffect IEquippedEffectSource.CreateOrReuse(RogueObj equipment, IEquippedEffect effect)
        {
            return CriticalAmuletEquipped.effect;
        }

        private class Effect : BaseEquippedEffect, IValueEffect
        {
            float IValueEffect.Order => 0f;

            void IValueEffect.AffectValue(IKeyword keyword, EffectableValue value, RogueObj self)
            {
                if (keyword == StatsKw.ATK)
                {
                    // 会心率 +100%
                    value.SubValues[StatsKw.CriticalRate] += 1f;
                }
            }
        }
    }
}
