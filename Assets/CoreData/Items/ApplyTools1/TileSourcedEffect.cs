using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    public class TileSourcedEffect
    {
        public static IRogueTile GetSource(RogueObj obj)
        {
            if (obj.Main.RogueEffects.TryGetEffect<Effect>(out var effect))
            {
                return effect.source;
            }
            return null;
        }

        public static void SetSource(RogueObj obj, IRogueTile source)
        {
            if (!obj.Main.RogueEffects.TryGetEffect<Effect>(out var effect))
            {
                effect = new Effect();
                obj.Main.RogueEffects.AddOpen(obj, effect);
            }
            effect.source = source;
        }

        [Objforming.Formable]
        private class Effect : IRogueEffect, IStatusEffect
        {
            public IRogueTile source;

            string IRogueDescription.Name => null;
            Sprite IRogueDescription.Icon => null;
            Color IRogueDescription.Color => Color.white;
            string IRogueDescription.Caption => null;
            IRogueDetails IRogueDescription.Details => null;

            IKeyword IStatusEffect.EffectCategory => EffectCategoryKw.Dummy;
            RogueObj IStatusEffect.Effecter => null;
            ISpriteMotion IStatusEffect.HeadIcon => null;
            float IStatusEffect.Order => 0f;

            void IRogueEffect.Open(RogueObj self)
            {
                RogueEffectUtility.AddFromRogueEffect(self, this);
            }

            void IStatusEffect.GetEffectedName(RogueNameBuilder refName, RogueObj self)
            {
                refName.Append(" [");
                refName.Append(source.Info.Name);
                refName.Append("]");
            }

            bool IRogueEffect.CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
            {
                return other is Effect effect && effect.source.Equals(source);
            }

            IRogueEffect IRogueEffect.DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
            {
                return new Effect() { source = source };
            }

            IRogueEffect IRogueEffect.ReplaceCloned(RogueObj obj, RogueObj clonedObj)
            {
                return this;
            }
        }
    }
}
