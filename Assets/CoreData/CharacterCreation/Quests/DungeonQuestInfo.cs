namespace Roguegard.CharacterCreation
{
    // ダンジョン自体はクエスト = null でも入れるようにしたい
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
        /// 上書き不可
        /// </summary>
        public static void SetTo(RogueObj self, DungeonQuest quest)
        {
            if (!self.TryGet<Info>(out var info))
            {
                info = new Info();
                self.SetInfo(info);
            }

            // 上書き不可
            if (info.quest != null) throw new System.InvalidOperationException();

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

            public bool CanStack(IRogueObjInfo coming) => false;
            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueObjInfo ReplaceObj(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
