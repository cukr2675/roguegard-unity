using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class ValueEffectState
    {
        private readonly List<IValueEffect> effects;

        private static readonly StaticInitializable<bool> recursion = new StaticInitializable<bool>(() => false);

        public ValueEffectState()
        {
            effects = new List<IValueEffect>();
        }

        public void AddFromInfoSet(RogueObj self, IValueEffect effect)
        {
            if (self.Main.RogueEffectOpenState == RogueEffectOpenState.OpeningEffects) throw new RogueException();
            if (effect == null) throw new System.ArgumentNullException(nameof(effect));

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

        public void AddFromRogueEffect(RogueObj self, IValueEffect effect)
        {
            if (self.Main.RogueEffectOpenState == RogueEffectOpenState.OpeningInfoSet) throw new RogueException();
            if (effect == null) throw new System.ArgumentNullException(nameof(effect));

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

        public bool Remove(IValueEffect effect)
        {
            return effects.Remove(effect);
        }

        public static void AffectValue(IKeyword keyword, EffectableValue value, RogueObj self)
        {
            if (recursion.Value)
            {
                Debug.LogError($"{nameof(AffectValue)} の再帰呼び出しは禁止です。");
                return;
            }
            recursion.Value = true;

            var valueEffectState = self.Main.GetValueEffectState(self);
            foreach (var effect in valueEffectState.effects)
            {
                effect.AffectValue(keyword, value, self);
            }

            recursion.Value = false;
        }
    }
}
