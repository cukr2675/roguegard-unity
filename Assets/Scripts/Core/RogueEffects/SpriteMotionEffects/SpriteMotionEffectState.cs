using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    public class SpriteMotionEffectState
    {
        private readonly List<ISpriteMotionEffect> bufferEffects;
        private readonly List<ISpriteMotionEffect> showEffects;
        private bool showEffectsIsDirty;

        public SpriteMotionEffectState()
        {
            bufferEffects = new List<ISpriteMotionEffect>();
            showEffects = new List<ISpriteMotionEffect>();
            showEffectsIsDirty = true;
        }

        public void AddFromInfoSet(RogueObj self, ISpriteMotionEffect effect)
        {
            if (self.Main.RogueEffectOpenState == RogueEffectOpenState.OpeningEffects) throw new RogueException();
            if (effect == null) throw new System.ArgumentNullException(nameof(effect));

            showEffectsIsDirty = true;
            for (int i = 0; i < bufferEffects.Count; i++)
            {
                // 同じ Order の要素が存在するときその手前に追加する。
                if (bufferEffects[i].Order >= effect.Order)
                {
                    bufferEffects.Insert(i, effect);
                    return;
                }
            }
            bufferEffects.Add(effect);
        }

        public void AddFromRogueEffect(RogueObj self, ISpriteMotionEffect effect)
        {
            if (self.Main.RogueEffectOpenState == RogueEffectOpenState.OpeningInfoSet) throw new RogueException();
            if (effect == null) throw new System.ArgumentNullException(nameof(effect));

            showEffectsIsDirty = true;
            for (int i = bufferEffects.Count - 1; i >= 0; i--)
            {
                // 同じ Order の要素が存在するときその後ろに追加する。
                if (bufferEffects[i].Order <= effect.Order)
                {
                    bufferEffects.Insert(i + 1, effect);
                    return;
                }
            }
            bufferEffects.Insert(0, effect);
        }

        public bool Remove(ISpriteMotionEffect effect)
        {
            var result = bufferEffects.Remove(effect);
            if (result) { showEffectsIsDirty = true; }
            return result;
        }

        public void Update()
        {
            if (!showEffectsIsDirty) return;

            showEffects.Clear();
            foreach (var effect in bufferEffects)
            {
                showEffects.Add(effect);
            }
            showEffectsIsDirty = false;
        }

        public void ApplyTo(
            ISpriteMotionSet motionSet, IKeyword keyword, int animationTime, RogueDirection direction, ref SkeletalSpriteTransform transform)
        {
            for (int i = 0; i < showEffects.Count; i++)
            {
                var motionEffect = showEffects[i];
                motionEffect.ApplyTo(motionSet, keyword, animationTime, direction, ref transform);
            }
        }
    }
}
