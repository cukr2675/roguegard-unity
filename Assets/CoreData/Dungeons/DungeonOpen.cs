using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class DungeonOpen : ReferableScript, IOpenEffect
    {
        public IRaceOption Open(RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            // ダンジョンの階層データを公開する
            var dungeon = (DungeonCreationData)characterCreationData;
            DungeonInfo.SetLevelsTo(self, dungeon.Levels, dungeon.LevelType, dungeon.VisibleRadius);
            return raceOption;
        }

        public void Close(RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
        }

        public IRaceOption Reopen(RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return raceOption;
        }

        public void InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
        }
    }
}
