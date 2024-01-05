using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [ObjectFormer.IgnoreRequireRelationalComponent]
    public class RogueBehaviourNodeEffect : IRogueEffect, IRogueObjUpdater
    {
        private RogueBehaviourNodeList node;

        float IRogueObjUpdater.Order => 1f;

        public static bool HasBehaviourNode(RogueObj obj)
        {
            var effects = obj.Main.RogueEffects.Effects;
            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i] is RogueBehaviourNodeEffect) return true;
            }
            return false;
        }

        public static void SetBehaviourNode(RogueObj obj, RogueBehaviourNodeList node)
        {
            if (node == null) throw new System.ArgumentNullException(nameof(node));

            if (!obj.Main.RogueEffects.TryGetEffect<RogueBehaviourNodeEffect>(out var effect))
            {
                effect = new RogueBehaviourNodeEffect();
                obj.Main.RogueEffects.AddOpen(obj, effect);
            }
            effect.node = node;
        }

        public static bool RemoveBehaviourNode(RogueObj obj)
        {
            if (!obj.Main.RogueEffects.TryGetEffect<RogueBehaviourNodeEffect>(out var effect))
            {
                RogueEffectUtility.RemoveClose(obj, effect);
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

        public bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other) => false;
        public IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
        public IRogueEffect ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
    }
}
