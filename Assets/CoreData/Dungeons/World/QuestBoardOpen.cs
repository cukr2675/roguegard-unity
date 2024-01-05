using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard
{
    public class QuestBoardOpen : ReferableScript, IOpenEffect
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
            public float Order => 100f;

            public RogueObjUpdaterContinueType UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
            {
                var quests = QuestBoardInfo.GetQuestList(self);
                if (quests == null)
                {
                    quests = QuestBoardInfo.SetQuestListTo(self);
                }

                var random = RogueRandom.Primary;
                while (quests.Count < 4)
                {
                    var quest = RoguegardSettings.DungeonQuestGenerator.GenerateQuest(random);
                    quests.Add(quest);
                }
                return default;
            }
        }
    }
}
