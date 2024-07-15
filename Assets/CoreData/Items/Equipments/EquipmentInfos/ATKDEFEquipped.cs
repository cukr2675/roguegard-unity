using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    /// <summary>
    /// <see cref="EquipmentCreationData"/> の攻撃力と防御力を参照して、装備者にバフをかける。
    /// </summary>
    public class ATKDEFEquipped : ReferableScript, IEquippedEffectSource
    {
        private ATKDEFEquipped() { }

        IEquippedEffect IEquippedEffectSource.CreateOrReuse(RogueObj equipment, IEquippedEffect effect)
        {
            if (effect is Effect effect1 && effect1.equipment == equipment) return effect1;
            else return new Effect() { equipment = equipment };
        }

        private class Effect : BaseEquippedEffect, IValueEffect
        {
            public RogueObj equipment;

            float IValueEffect.Order => 0f;

            void IValueEffect.AffectValue(IKeyword keyword, EffectableValue value, RogueObj self)
            {
                if (keyword == StatsKw.ATK)
                {
                    value.MainValue += equipment.Main.InfoSet.ATK;
                }
                if (keyword == StatsKw.DEF)
                {
                    value.MainValue -= equipment.Main.InfoSet.DEF;
                }
            }
        }
    }
}
