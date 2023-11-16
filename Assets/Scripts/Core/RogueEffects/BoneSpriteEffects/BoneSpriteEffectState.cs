using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class BoneSpriteEffectState
    {
        private readonly List<IBoneSpriteEffect> effects;

        private bool isDirty;

        public BoneSpriteEffectState()
        {
            effects = new List<IBoneSpriteEffect>();
            isDirty = true;
        }

        public void AddFromInfoSet(RogueObj self, IBoneSpriteEffect effect)
        {
            if (self.Main.RogueEffectOpenState == RogueEffectOpenState.OpeningEffects) throw new RogueException();
            if (effect == null) throw new System.ArgumentNullException(nameof(effect));

            isDirty = true;
            for (int i = 0; i < effects.Count; i++)
            {
                // 同じ Order の要素が存在するときその手前に追加する。
                if (effects[i].Order >= effect.Order)
                {
                    effects.Insert(i, effect);
                    return;
                }
            }
            effects.Add(effect);
        }

        public void AddFromRogueEffect(RogueObj self, IBoneSpriteEffect effect)
        {
            if (self.Main.RogueEffectOpenState == RogueEffectOpenState.OpeningInfoSet) throw new RogueException();
            if (effect == null) throw new System.ArgumentNullException(nameof(effect));

            isDirty = true;
            for (int i = effects.Count - 1; i >= 0; i--)
            {
                // 同じ Order の要素が存在するときその後ろに追加する。
                if (effects[i].Order <= effect.Order)
                {
                    effects.Insert(i + 1, effect);
                    return;
                }
            }
            effects.Insert(0, effect);
        }

        public bool Remove(IBoneSpriteEffect effect)
        {
            var result = effects.Remove(effect);
            if (result) { isDirty = true; }
            return result;
        }

        internal void SetTo(RogueObj self, RogueObjSprite selfSprite, bool forced)
        {
            if (!forced && !isDirty) return;

            selfSprite?.SetBoneSpriteEffects(self, effects);
            isDirty = false;
        }
    }
}
