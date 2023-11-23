using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public abstract class AbilityIntrinsicOptionScript : ScriptIntrinsicOption.Script
    {
        public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new AbilitySortedIntrinsic(this, lv);
        }

        protected virtual void LevelUpToLv(RogueObj self, MainInfoSetType infoSetType)
        {
            RogueEffectUtility.AddFromInfoSet(self, this);
        }

        protected virtual void LevelDownFromLv(RogueObj self, MainInfoSetType infoSetType)
        {
            RogueEffectUtility.Remove(self, this);
        }

        private class AbilitySortedIntrinsic : ISortedIntrinsic
        {
            private readonly AbilityIntrinsicOptionScript option;
            public int Lv { get; }

            public AbilitySortedIntrinsic(AbilityIntrinsicOptionScript option, int lv)
            {
                this.option = option;
                Lv = lv;
            }

            void ISortedIntrinsic.LevelUpToLv(RogueObj self, MainInfoSetType infoSetType)
            {
                if (infoSetType != self.Main.InfoSetState) return;

                option.LevelUpToLv(self, infoSetType);
            }

            void ISortedIntrinsic.LevelDownFromLv(RogueObj self, MainInfoSetType infoSetType)
            {
                if (infoSetType != self.Main.InfoSetState) return;

                option.LevelDownFromLv(self, infoSetType);
            }

            void ISortedIntrinsic.Open(RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base)
            {
                if (self.Main.Stats.Lv >= Lv)
                {
                    option.LevelUpToLv(self, infoSetType);
                }
            }

            void ISortedIntrinsic.Close(RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph)
            {
                if (self.Main.Stats.Lv >= Lv)
                {
                    option.LevelDownFromLv(self, infoSetType);
                }
            }
        }
    }
}
