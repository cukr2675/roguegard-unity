using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public static class DungeonQuestInfo
    {
        public static bool TryGetQuest(RogueObj self, out DungeonQuest quest)
        {
            if (self.TryGet<Info>(out var info))
            {
                quest = info.quest;
                return true;
            }
            else
            {
                quest = null;
                return false;
            }
        }

        /// <summary>
        /// è„èëÇ´ïsâ¬
        /// </summary>
        public static void SetTo(RogueObj self, DungeonQuest quest)
        {
            if (!self.TryGet<Info>(out var info))
            {
                info = new Info();
                self.SetInfo(info);
            }

            // è„èëÇ´ïsâ¬
            if (info.quest != null) throw new RogueException();

            info.quest = quest;
        }

        public static bool RemoveFrom(RogueObj self)
        {
            return self.RemoveInfo(typeof(Info));
        }

        [Objforming.IgnoreRequireRelationalComponent]
        private class Info : IRogueObjInfo
        {
            public DungeonQuest quest;

            public bool IsExclusedWhenSerialize => true;

            public bool CanStack(IRogueObjInfo other) => false;
            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
