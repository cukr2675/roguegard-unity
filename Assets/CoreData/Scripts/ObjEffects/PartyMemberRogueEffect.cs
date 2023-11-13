using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public abstract class PartyMemberRogueEffect : IRogueEffect, IRogueObjUpdater
    {
        float IRogueObjUpdater.Order => UpdaterOrder;
        protected virtual float UpdaterOrder => 100f;

        void IRogueEffect.Open(RogueObj self) => Open(self);
        protected virtual void Open(RogueObj self)
        {
            RogueEffectUtility.AddFromRogueEffect(self, this);
        }

        RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
        {
            if (sectionIndex == 0 && !ContainsEffecter())
            {
                // パーティから付与能力を持つキャラがいなくなったら解除する。
                RogueEffectUtility.RemoveClose(self, this);
                return default;
            }

            return UpdateObj(self, activationDepth, ref sectionIndex);

            bool ContainsEffecter()
            {
                if (self.Main.Stats.Party == null)
                {
                    return MemberIsEffecter(self);
                }
                else
                {
                    var partyMembers = self.Main.Stats.Party.Members;
                    for (int i = 0; i < partyMembers.Count; i++)
                    {
                        if (MemberIsEffecter(partyMembers[i])) return true;
                    }
                    return false;
                }
            }
        }

        protected abstract bool MemberIsEffecter(RogueObj partyMember);

        protected virtual RogueObjUpdaterContinueType UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
        {
            return default;
        }

        bool IRogueEffect.CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other) => false;
        IRogueEffect IRogueEffect.DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => this;
        IRogueEffect IRogueEffect.ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
    }
}
