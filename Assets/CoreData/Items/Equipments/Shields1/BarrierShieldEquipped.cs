using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class BarrierShieldEquipped : ReferableScript, IEquippedEffectSource
    {
        private static readonly Effect effect1 = new Effect();

        IEquippedEffect IEquippedEffectSource.CreateOrReuse(RogueObj equipment, IEquippedEffect effect)
        {
            return effect1;
        }

        private class Effect : BaseEquippedEffect, IValueEffect
        {
            float IValueEffect.Order => AttackUtility.BaseCupValueEffectOrder;

            void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
            {
                if (keyword == StatsKw.DEF)
                {
                    if (self.Main.Stats.MP >= 5)
                    {
                        // MP が 5 以上のとき 35% でガード
                        value.SubValues[StatsKw.GuardRate] = AttackUtility.Cup(value.SubValues[StatsKw.GuardRate], 0.35f);
                    }
                }
            }
        }
    }
}
