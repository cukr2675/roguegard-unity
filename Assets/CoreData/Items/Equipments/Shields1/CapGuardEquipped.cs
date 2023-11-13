using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class CapGuardEquipped : ReferableScript, IEquippedEffectSource
    {
        [SerializeField] private float _guardRate = 0f;

        private Effect effect1;

        IEquippedEffect IEquippedEffectSource.CreateOrReuse(RogueObj equipment, IEquippedEffect effect)
        {
            if (effect1 == null) { effect1 = new Effect() { parent = this }; }
            return effect1;
        }

        private class Effect : BaseEquippedEffect, IValueEffect
        {
            public CapGuardEquipped parent;

            float IValueEffect.Order => 0f;

            void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
            {
                if (keyword == StatsKw.DEF)
                {
                    value.SubValues[StatsKw.GuardRate] = AttackUtility.Cup(value.SubValues[StatsKw.GuardRate], parent._guardRate);
                }
            }
        }
    }
}
