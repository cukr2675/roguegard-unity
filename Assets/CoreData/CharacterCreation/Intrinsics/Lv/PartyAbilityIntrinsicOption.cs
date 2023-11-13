using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public abstract class PartyAbilityIntrinsicOption : BaseIntrinsicOption
    {
        protected abstract class AbilitySortedIntrinsic : ISortedIntrinsic, IStatusEffect
        {
            public int Lv { get; }

            // IRogueEffect でないエフェクトを Contains で判定したいので、 IStatusEffect として追加してそちらの Contains を使う。
            IKeyword IStatusEffect.EffectCategory => EffectCategoryKw.Dummy;
            RogueObj IStatusEffect.Effecter => null;
            IBoneMotion IStatusEffect.HeadIcon => null;
            float IStatusEffect.Order => 0f;
            string IRogueDescription.Name => null;
            Sprite IRogueDescription.Icon => null;
            Color IRogueDescription.Color => Color.white;
            string IRogueDescription.Caption => null;
            object IRogueDescription.Details => null;

            protected AbilitySortedIntrinsic(int lv)
            {
                Lv = lv;
            }

            public virtual void LevelUpToLv(RogueObj self, MainInfoSetType infoSetType)
            {
                RogueEffectUtility.AddFromInfoSet(self, this);
            }

            public virtual void LevelDownFromLv(RogueObj self, MainInfoSetType infoSetType)
            {
                RogueEffectUtility.Remove(self, this);
            }

            void ISortedIntrinsic.Open(RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base)
            {
                if (self.Main.Stats.Lv >= Lv)
                {
                    LevelUpToLv(self, infoSetType);
                }
            }

            void ISortedIntrinsic.Close(RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph)
            {
                if (self.Main.Stats.Lv >= Lv)
                {
                    LevelDownFromLv(self, infoSetType);
                }
            }

            public virtual void GetEffectedName(RogueNameBuilder refName, RogueObj self) { }
        }

        protected abstract class BasePartyMemberRogueEffect<T> : PartyMemberRogueEffect
            where T : IStatusEffect
        {
            protected sealed override bool MemberIsEffecter(RogueObj partyMember)
            {
                var statusEffectState = partyMember.Main.GetStatusEffectState(partyMember);
                return statusEffectState.TryGetStatusEffect<T>(out _);
            }
        }
    }
}
