using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class OnChangeGederOpen : ReferableScript, IOpenEffect
    {
        [SerializeField] private ScriptableCharacterCreationData _to = null;

        public IRaceOption Open(
            RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            if (self.Main.RogueEffectOpenState == RogueEffectOpenState.Finished)
            {
                return Reopen(self, infoSetType, raceOption, characterCreationData);
            }
            else
            {
                return raceOption;
            }
        }

        public void Close(
            RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
        }

        public IRaceOption Reopen(
            RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            if (self.Main.InfoSet is CharacterCreationInfoSet infoSet && infoSet.CurrentRaceOption != raceOption)
            {
                // self.Main.InfoSet 相当であるときだけ進化処理をする。
                return raceOption;
            }

            if (ChangeGenderEffect.GetChanged(self))
            {
                ChangeGenderEffect.Change(self);
                return _to.Race.Option;
            }
            return raceOption;
        }

        public void InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
        }
    }
}
