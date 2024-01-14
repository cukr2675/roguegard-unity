using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class RandomWalkOpen : ReferableScript, IOpenEffect
    {
        private static readonly Effect effect = new Effect();

        public IRaceOption Open(
            RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            RogueEffectUtility.AddFromInfoSet(self, effect);
            return raceOption;
        }

        public void Close(
            RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            RogueEffectUtility.Remove(self, effect);
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
            float IRogueObjUpdater.Order => 1f;

            RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
            {
                RogueObjUpdaterContinueType result;
                if (sectionIndex == 0)
                {
                    result = TickUtility.Section0Update(self, ref sectionIndex);
                }
                else
                {
                    Update(activationDepth);
                    result = TickUtility.SectionAfter1LateUpdate(self, ref sectionIndex);
                }
                return result;

                void Update(float activationDepth)
                {
                    var direction = new RogueDirection(RogueRandom.Primary.Next(0, 8));
                    if (MovementUtility.TryGetApproachDirection(self, self.Position + direction.Forward, true, out var approachDirection))
                    {
                        this.Walk(self, approachDirection, activationDepth);
                    }
                }
            }
        }
    }
}
