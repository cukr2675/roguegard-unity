using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    public static class ChangeGenderEffect
    {
        private static readonly Effect effect = new Effect();

        public static bool GetChanged(RogueObj obj)
        {
            return obj.Main.RogueEffects.Contains(effect);
        }

        public static void Change(RogueObj obj)
        {
            if (obj.Main.RogueEffects.Contains(effect))
            {
                // 性転換
                obj.Main.RogueEffects.AddOpen(obj, effect);
                obj.Main.Polymorph(obj, obj.Main.InfoSet);
            }
            else
            {
                // 性転換解除
                RogueEffectUtility.RemoveClose(obj, effect);
                obj.Main.Polymorph(obj, obj.Main.InfoSet);
            }
        }

        [ObjectFormer.Formable]
        private class Effect : IRogueEffect, IStatusEffect, IValueEffect
        {
            float IValueEffect.Order => 0f;

            string IRogueDescription.Name => "性転換";
            Sprite IRogueDescription.Icon => null;
            Color IRogueDescription.Color => Color.white;
            string IRogueDescription.Caption => null;
            IRogueDetails IRogueDescription.Details => null;
            IKeyword IStatusEffect.EffectCategory => null;
            RogueObj IStatusEffect.Effecter => null;
            IBoneMotion IStatusEffect.HeadIcon => null;
            float IStatusEffect.Order => 0f;

            void IRogueEffect.Open(RogueObj self)
            {
                RogueEffectUtility.AddFromRogueEffect(self, this);
            }

            void IValueEffect.AffectValue(IKeyword keyword, AffectableValue value, RogueObj self)
            {
                if (keyword == StatsKw.Gender)
                {
                    var selfIsMale = value.SubValues.Is(StatsKw.Male);
                    var selfIsFemale = value.SubValues.Is(StatsKw.Female);
                    if (selfIsMale && (!selfIsFemale))
                    {
                        // 男性なら女性にする。
                        value.SubValues[StatsKw.Female] = 1f;
                        value.SubValues[StatsKw.LooksFemale] = 1f;
                        value.SubValues[StatsKw.Male] = 0f;
                        value.SubValues[StatsKw.LooksMale] = 0f;
                    }
                    if (selfIsFemale && (!selfIsMale))
                    {
                        // 女性なら男性にする。
                        value.SubValues[StatsKw.Male] = 1f;
                        value.SubValues[StatsKw.LooksMale] = 1f;
                        value.SubValues[StatsKw.Female] = 0f;
                        value.SubValues[StatsKw.LooksFemale] = 0f;
                    }
                }
            }

            void IStatusEffect.GetEffectedName(RogueNameBuilder refName, RogueObj self) { }

            bool IRogueEffect.CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other) => other == this;
            IRogueEffect IRogueEffect.DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => this;
            IRogueEffect IRogueEffect.ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
            public override bool Equals(object obj) => obj.GetType() == GetType();
            public override int GetHashCode() => GetType().GetHashCode();
        }
    }
}
