using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class RogueBehaviourNodeEffect : IRogueObjUpdater, IStatusEffect
    {
        private RogueBehaviourNodeList node;
        private float priority;

        IKeyword IStatusEffect.EffectCategory => EffectCategoryKw.Dummy;
        RogueObj IStatusEffect.Effecter => null;
        IBoneMotion IStatusEffect.HeadIcon => null;
        string IRogueDescription.Name => null;
        Sprite IRogueDescription.Icon => null;
        Color IRogueDescription.Color => Color.white;
        string IRogueDescription.Caption => null;
        IRogueDetails IRogueDescription.Details => null;
        float IStatusEffect.Order => 0f;

        float IRogueObjUpdater.Order => 1f;

        public static float GetPriority(RogueObj obj)
        {
            var statusEffectState = obj.Main.GetStatusEffectState(obj);
            if (statusEffectState.TryGetStatusEffect<RogueBehaviourNodeEffect>(out var effect))
            {
                return effect.priority;
            }
            else
            {
                return float.NegativeInfinity;
            }
        }

        public static void SetBehaviourNode(RogueObj obj, RogueBehaviourNodeList node, float priority)
        {
            if (node == null) throw new System.ArgumentNullException(nameof(node));

            var statusEffectState = obj.Main.GetStatusEffectState(obj);
            if (!statusEffectState.TryGetStatusEffect<RogueBehaviourNodeEffect>(out var effect))
            {
                effect = new RogueBehaviourNodeEffect();
                RogueEffectUtility.AddFromRogueEffect(obj, effect);
            }
            if (priority <= effect.priority) throw new RogueException(
                $"新しいビヘイビアの優先度 ({priority}) は既存のビヘイビアの優先度 ({effect.priority}) 以下のため上書きできません。");

            effect.node = node;
            effect.priority = priority;
        }

        public static bool RemoveBehaviourNode(RogueObj obj, float priority)
        {
            var statusEffectState = obj.Main.GetStatusEffectState(obj);
            if (statusEffectState.TryGetStatusEffect<RogueBehaviourNodeEffect>(out var effect) &&
                effect.priority == priority)
            {
                RogueEffectUtility.Remove(obj, effect);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Open(RogueObj self)
        {
            RogueEffectUtility.AddFromRogueEffect(self, this);
        }

        RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
        {
            if (self.Location?.Space.Tilemap == null) return default;

            RogueObjUpdaterContinueType result;
            if (sectionIndex == 0)
            {
                result = TickUtility.Section0Update(self, ref sectionIndex);
            }
            else
            {
                node.Tick(self, activationDepth);
                result = TickUtility.SectionAfter1LateUpdate(self, ref sectionIndex);
            }
            return result;
        }

        void IStatusEffect.GetEffectedName(RogueNameBuilder refName, RogueObj self)
        {
        }
    }
}
