using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class QuestBoardInfo
    {
        public DungeonQuestTable QuestTable { get; set; }

        public int MinPartySize { get; set; }
        public int MaxPartySize { get; set; }
        public int WeightTurnsAfterAccept { get; set; }

        private QuestBoardInfo()
        {
            QuestTable = new DungeonQuestTable();
            MinPartySize = 1;
            MaxPartySize = 2;
            WeightTurnsAfterAccept = 10;
        }

        [ObjectFormer.CreateInstance]
        private QuestBoardInfo(bool flag) { }

        public static QuestBoardInfo Get(RogueObj obj)
        {
            if (obj.TryGet<Info>(out var info))
            {
                return info.info;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 上書き不可
        /// </summary>
        public static QuestBoardInfo SetTo(RogueObj obj)
        {
            if (!obj.TryGet<Info>(out _))
            {
                var info = new Info();
                obj.SetInfo(info);
                return info.info;
            }
            else
            {
                // 上書き不可
                throw new RogueException();
            }
        }

        /// <summary>
        /// この <see cref="QuestBoardInfo"/> の設定をもとに自動でパーティに参加する
        /// </summary>
        public bool TryAutoAssign(RogueObj obj)
        {
            for (int i = 0; i < QuestTable.Count; i++)
            {
                QuestTable.GetItem(i, out _, out var party, out _);
                if (party == null || party.Members.Count >= MaxPartySize) continue;

                if (obj.Main.Stats.TryAssignParty(obj, party))
                {
                    if (party.Members.Count == MinPartySize) { QuestTable.TryAcceptAt(i, party, WeightTurnsAfterAccept); }
                    return true;
                }
            }
            return false;
        }

        [ObjectFormer.Formable]
        private class Info : IRogueObjInfo
        {
            public QuestBoardInfo info = new QuestBoardInfo();

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo other) => false;
            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
