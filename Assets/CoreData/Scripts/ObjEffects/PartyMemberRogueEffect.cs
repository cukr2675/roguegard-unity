namespace Roguegard
{
    public abstract class PartyMemberRogueEffect : IRogueEffect, IRogueObjUpdater
    {
        float IRogueObjUpdater.Order => UpdaterOrder;
        protected virtual float UpdaterOrder => 100f;

        public void AffectToPartyMembersOf(RogueObj self, bool affectToSelf)
        {
            if (self.Main.Stats.Party == null)
            {
                Add(self);
            }
            else
            {
                foreach (var partyMember in self.Main.Stats.Party.Members)
                {
                    Add(partyMember);
                }
            }

            void Add(RogueObj partyMember)
            {
                if (!affectToSelf && partyMember == self) return;
                if (partyMember.Main.RogueEffects.TryGetEffect<PartyMemberRogueEffect>(GetType(), out _)) return;

                partyMember.Main.RogueEffects.AddOpen(partyMember, this);
            }
        }

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
                    foreach (var partyMember in self.Main.Stats.Party.Members)
                    {
                        if (MemberIsEffecter(partyMember)) return true;
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

        bool IRogueEffect.CanStack(RogueObj self, RogueObj comingObj, IRogueEffect coming) => false;
        IRogueEffect IRogueEffect.DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => this;
        IRogueEffect IRogueEffect.ReplaceObj(RogueObj obj, RogueObj clonedObj) => this;
    }
}
