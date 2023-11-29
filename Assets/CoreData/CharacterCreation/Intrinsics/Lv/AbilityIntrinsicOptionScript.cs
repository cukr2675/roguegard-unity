using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public abstract class AbilityIntrinsicOptionScript : ScriptIntrinsicOption.Script
    {
        protected class AbilitySortedIntrinsic : ISortedIntrinsic
        {
            public int Lv { get; }

            public AbilitySortedIntrinsic(int lv)
            {
                Lv = lv;
            }

            protected virtual void LevelUpToLv(RogueObj self, MainInfoSetType infoSetType)
            {
                RogueEffectUtility.AddFromInfoSet(self, this);
            }

            protected virtual void LevelDownFromLv(RogueObj self, MainInfoSetType infoSetType)
            {
                RogueEffectUtility.Remove(self, this);
            }

            void ISortedIntrinsic.LevelUpToLv(RogueObj self, MainInfoSetType infoSetType)
            {
                if (infoSetType != self.Main.InfoSetState) return;

                LevelUpToLv(self, infoSetType);
            }

            void ISortedIntrinsic.LevelDownFromLv(RogueObj self, MainInfoSetType infoSetType)
            {
                if (infoSetType != self.Main.InfoSetState) return;

                LevelDownFromLv(self, infoSetType);
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
        }
    }
}
