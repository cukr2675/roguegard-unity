using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class RogueEffectUtility
    {
        /// <summary>
        /// <paramref name="effect"/> に実装されているエフェクトのインターフェースを <paramref name="self"/> に追加する。
        /// </summary>
        public static void AddFromRogueEffect(RogueObj self, object effect)
        {
            if (effect is IStatusEffect statusEffect)
            {
                var statusEffectState = self.Main.GetStatusEffectState(self);
                statusEffectState.AddFromRogueEffect(self, statusEffect);
            }
            if (effect is IValueEffect valueEffect)
            {
                var valueEffectState = self.Main.GetValueEffectState(self);
                valueEffectState.AddFromRogueEffect(self, valueEffect);
            }
            if (effect is IRogueObjUpdater updater)
            {
                var updaterState = self.Main.GetRogueObjUpdaterState(self);
                updaterState.AddFromRogueEffect(self, updater);
            }
            if (effect is IRogueMethodActiveAspect activeAspect)
            {
                var aspectState = self.Main.GetRogueMethodAspectState(self);
                aspectState.AddActiveFromRogueEffect(self, activeAspect);
            }
            if (effect is IRogueMethodPassiveAspect passiveAspect)
            {
                var aspectState = self.Main.GetRogueMethodAspectState(self);
                aspectState.AddPassiveFromRogueEffect(self, passiveAspect);
            }
            if (effect is IBoneSpriteEffect equipmentSpriteEffect)
            {
                var equipmentSpriteState = self.Main.GetBoneSpriteEffectState(self);
                equipmentSpriteState.AddFromRogueEffect(self, equipmentSpriteEffect);
            }
            if (effect is ISpriteMotionEffect motionEffect)
            {
                var motionEffectState = self.Main.GetSpriteMotionEffectState(self);
                motionEffectState.AddFromRogueEffect(self, motionEffect);
            }
        }

        public static void AddFromInfoSet(RogueObj self, object effect)
        {
            if (effect is IStatusEffect statusEffect)
            {
                var statusEffectState = self.Main.GetStatusEffectState(self);
                statusEffectState.AddFromInfoSet(self, statusEffect);
            }
            if (effect is IValueEffect valueEffect)
            {
                var valueEffectState = self.Main.GetValueEffectState(self);
                valueEffectState.AddFromInfoSet(self, valueEffect);
            }
            if (effect is IRogueObjUpdater updater)
            {
                var updaterState = self.Main.GetRogueObjUpdaterState(self);
                updaterState.AddFromInfoSet(self, updater);
            }
            if (effect is IRogueMethodActiveAspect activeAspect)
            {
                var aspectState = self.Main.GetRogueMethodAspectState(self);
                aspectState.AddActiveFromInfoSet(self, activeAspect);
            }
            if (effect is IRogueMethodPassiveAspect passiveAspect)
            {
                var aspectState = self.Main.GetRogueMethodAspectState(self);
                aspectState.AddPassiveFromInfoSet(self, passiveAspect);
            }
            if (effect is IBoneSpriteEffect equipmentSpriteEffect)
            {
                var equipmentSpriteState = self.Main.GetBoneSpriteEffectState(self);
                equipmentSpriteState.AddFromInfoSet(self, equipmentSpriteEffect);
            }
            if (effect is ISpriteMotionEffect motionEffect)
            {
                var motionEffectState = self.Main.GetSpriteMotionEffectState(self);
                motionEffectState.AddFromInfoSet(self, motionEffect);
            }
        }

        /// <summary>
        /// <paramref name="effect"/> に実装されているエフェクトのインターフェースを <paramref name="self"/> から取り除く。
        /// </summary>
        public static bool Remove(RogueObj self, object effect)
        {
            if (effect is IRogueEffect)
            {
                Debug.LogWarning($"取り除くエフェクトが {nameof(IRogueEffect)} です。");
            }

            return RemoveOther(self, effect);
        }

        /// <summary>
        /// <paramref name="effect"/> に実装されているエフェクトのインターフェースを <paramref name="self"/> から取り除く。
        /// <see cref="IRogueEffect"/> も解除する。
        /// </summary>
        public static bool RemoveClose(RogueObj self, IRogueEffect effect)
        {
            var result = false;
            result |= self.Main.RogueEffects.Remove(effect);
            result |= RemoveOther(self, effect);
            return result;
        }

        private static bool RemoveOther(RogueObj self, object effect)
        {
            var result = false;
            if (effect is IStatusEffect statueEffect)
            {
                var statusEffectState = self.Main.GetStatusEffectState(self);
                result |= statusEffectState.Remove(statueEffect);
            }
            if (effect is IValueEffect valueEffect)
            {
                var valueEffectState = self.Main.GetValueEffectState(self);
                result |= valueEffectState.Remove(valueEffect);
            }
            if (effect is IRogueObjUpdater updater)
            {
                var updaterState = self.Main.GetRogueObjUpdaterState(self);
                result |= updaterState.ReplaceWithNull(updater);
            }
            if (effect is IRogueMethodActiveAspect activeAspect)
            {
                var aspectState = self.Main.GetRogueMethodAspectState(self);
                result |= aspectState.ReplaceActiveWithNull(activeAspect);
            }
            if (effect is IRogueMethodPassiveAspect passiveAspect)
            {
                var aspectState = self.Main.GetRogueMethodAspectState(self);
                result |= aspectState.ReplacePassiveWithNull(passiveAspect);
            }
            if (effect is IBoneSpriteEffect equipmentSpriteEffect)
            {
                var equipmentSpriteState = self.Main.GetBoneSpriteEffectState(self);
                result |= equipmentSpriteState.Remove(equipmentSpriteEffect);
            }
            if (effect is ISpriteMotionEffect motionEffect)
            {
                var motionEffectState = self.Main.GetSpriteMotionEffectState(self);
                result |= motionEffectState.Remove(motionEffect);
            }
            return result;
        }
    }
}
