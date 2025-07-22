namespace Roguegard.CharacterCreation
{
    /// <summary>
    /// <see cref="EquipmentCreationDataAsset"/> の攻撃力と防御力を参照して、装備者にバフをかける。
    /// </summary>
    public class AtkDefEquipped : ReferableScript, IEquippedEffectSource
    {
        private AtkDefEquipped() { }

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
                if (keyword == StatsKw.Atk)
                {
                    value.MainValue += equipment.Main.InfoSet.Atk;
                }
                if (keyword == StatsKw.Def)
                {
                    value.MainValue -= equipment.Main.InfoSet.Def;
                }
            }
        }
    }
}
