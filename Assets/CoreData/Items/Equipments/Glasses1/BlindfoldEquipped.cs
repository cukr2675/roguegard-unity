using OchalikeSprites;
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
            string IRogueDescribable.Name => "盲目";
            Sprite IRogueDescribable.Icon => null;
            Color IRogueDescribable.Color => Color.white;
            string IRogueDescribable.Caption => null;
            IRogueDetails IRogueDescribable.Details => null;
            Spanning<IKeyword> IRogueDescribable.Tags => Spanning<IKeyword>.Empty;
            IKeyword IStatusEffect.EffectCategory => CategoryKw.Equipment;
            ISpriteMotion IStatusEffect.HeadIcon => null;
            float IStatusEffect.Order => 0f;

            public RogueObj Effecter { get; }
            float IValueEffect.Order => 0f;

            public Effect(RogueObj equipment)
            {
                Effecter = equipment;
            }

            void IValueEffect.AffectValue(IKeyword keyword, EffectableValue value, RogueObj self)
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
