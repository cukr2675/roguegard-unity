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
                var questBoardInfo = QuestBoardInfo.Get(self);
                if (questBoardInfo == null)
                {
                    questBoardInfo = QuestBoardInfo.SetTo(self);
                }
                var questTable = questBoardInfo.QuestTable;

                // クエストを4個そろえる
                var random = RogueRandom.Primary;
                while (questTable.Count < 4)
                {
                    var quest = RoguegardSettings.DungeonQuestGenerator.GenerateQuest(random);
                    questTable.Add(quest);
                }

                // 最少人数以上で待ち時間がゼロになったクエストは出発させる
                questTable.UpdateWeightTurns();
                for (int i = 0; i < questTable.Count; i++)
                {
                    questTable.GetItem(i, out var quest, out var party, out var weightTurns);
                    if (party == null || party.Members.Count < questBoardInfo.MinPartySize || weightTurns >= 1) continue;

                    quest.Start(party.Members[0]);
                    questTable.RemoveAt(i);
                    i--;
                }

                return default;
            }
        }
    }
}
