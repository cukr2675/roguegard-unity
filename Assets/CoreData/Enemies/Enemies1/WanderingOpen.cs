using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class WanderingOpen : ReferableScript, IOpenEffect
    {
        public IRaceOption Open(
            RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            Effect.SetTo(self);
            return raceOption;
        }

        public void Close(
            RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            Effect.RemoveFrom(self);
        }

        public IRaceOption Reopen(
            RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return raceOption;
        }

        public void InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
        }

        private class Effect : IRogueObjUpdater
        {
            private static readonly Effect instance = new Effect();

            float IRogueObjUpdater.Order => -priority;

            private const float priority = 1f;

            public static void SetTo(RogueObj obj)
            {
                RogueEffectUtility.AddFromInfoSet(obj, instance);
            }

            public static void RemoveFrom(RogueObj obj)
            {
                RogueEffectUtility.Remove(obj, instance);
                RogueBehaviourNodeEffect.RemoveBehaviourNode(obj, priority);
            }

            RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
            {
                var effectedPriority = RogueBehaviourNodeEffect.GetPriority(self);
                if (priority <= effectedPriority) return default;

                var node = new RogueBehaviourNodeList();
                node.Add(new FearMovementBehaviourNode());
                node.Add(new AttackBehaviourNode());
                node.Add(new SearchEnemyBehaviourNode());
                node.Add(new ChaseBehaviourNode());
                node.Add(new WanderingBehaviourNode());
                RogueBehaviourNodeEffect.SetBehaviourNode(self, node, priority);
                return default;
            }
        }
    }
}
