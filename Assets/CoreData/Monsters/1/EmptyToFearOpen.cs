using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class EmptyToFearOpen : ReferableScript, IOpenEffect
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

        private class Effect : IRogueObjUpdater, IRogueMethodActiveAspect
        {
            float IRogueObjUpdater.Order => 100f;
            float IRogueMethodActiveAspect.Order => 0f;

            public RogueObjUpdaterContinueType UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
            {
                if (self.Main.Stats.MP == 0)
                {
                    // 恐怖状態にする
                    this.Affect(self, activationDepth, FearStatusEffect.Callback);
                }
                return RogueObjUpdaterContinueType.Break;
            }

            bool IRogueMethodActiveAspect.ActiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj target, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.ActiveNext next)
            {
                var result = next.Invoke(keyword, method, self, target, activationDepth, arg);
                if (keyword == MainInfoKw.Skill && self.Main.Stats.MP == 0)
                {
                    // 恐怖状態にする
                    this.Affect(self, activationDepth, FearStatusEffect.Callback);

                    // スキル発動によって MP がゼロになった場合は再行動させる
                    // (Updater では ChargedSpeed を可算してもすでに行動済みなため)
                    self.Main.Stats.ChargedSpeed++;
                }
                return result;
            }
        }
    }
}
