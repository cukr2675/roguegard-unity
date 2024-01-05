using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard
{
    public static class QuestBoardInfo
    {
        public static DungeonQuestList GetQuestList(RogueObj obj)
        {
            if (obj.TryGet<Info>(out var info))
            {
                return info.Quests;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// è„èëÇ´ïsâ¬
        /// </summary>
        public static DungeonQuestList SetQuestListTo(RogueObj obj)
        {
            if (!obj.TryGet<Info>(out var info))
            {
                info = new Info();
                info.Quests = new DungeonQuestList();
                obj.SetInfo(info);
                return info.Quests;
            }
            else
            {
                // è„èëÇ´ïsâ¬
                throw new RogueException();
            }
        }

        [ObjectFormer.Formable]
        private class Info : IRogueObjInfo
        {
            public DungeonQuestList Quests { get; set; }

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo other) => false;
            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
