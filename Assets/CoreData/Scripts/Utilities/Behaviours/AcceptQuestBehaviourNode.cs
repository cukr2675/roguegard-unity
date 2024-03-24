using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class AcceptQuestBehaviourNode : IRogueBehaviourNode
    {
        private int count;

        private const int minCount = 10;
        private const int maxCount = 20;

        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            if (self.Location == null) return RogueObjUpdaterContinueType.Continue;

            var nearestQuestBoardInfo = GetNearestQuestBoardInfo(self);

            // クエストを受注済みなら待機する
            if (nearestQuestBoardInfo.QuestTable.Contains(self.Main.Stats.Party)) return RogueObjUpdaterContinueType.Break;

            if (count <= 0)
            {
                // 残りカウントが設定されていなければ、乱数でカウントを設定する
                count = RogueRandom.Primary.Next(minCount, maxCount);

                // 同時に現在のパーティから抜ける
                // 自分がリーダーのときはパーティを解体する
                var party = self.Main.Stats.Party;
                if (party != null)
                {
                    if (party.Members[0] == self)
                    {
                        while (party.Members.Count >= 1)
                        {
                            var member = party.Members[0];
                            var mainParty = RogueDevice.Primary.Player.Main.Stats.Party;
                            member.Main.Stats.TryAssignParty(member, new RogueParty(mainParty.Faction, mainParty.TargetFactions));
                        }
                    }
                    else
                    {
                        var mainParty = RogueDevice.Primary.Player.Main.Stats.Party;
                        self.Main.Stats.TryAssignParty(self, new RogueParty(mainParty.Faction, mainParty.TargetFactions));
                    }
                }
            }

            // カウントがゼロになる前に、同じクエストボードから受注済みのロビーメンバーがいたらそこに参加する
            if (nearestQuestBoardInfo.TryAutoAssign(self))
            {
                count = 0;
                return RogueObjUpdaterContinueType.Break;
            }

            // カウントがゼロになったときクエストを受注
            count--;
            if (count <= 0)
            {
                count = 0;
                nearestQuestBoardInfo.QuestTable.TryAcceptAt(0, self.Main.Stats.Party, nearestQuestBoardInfo.WeightTurnsAfterAccept);
                return RogueObjUpdaterContinueType.Break;
            }

            return RogueObjUpdaterContinueType.Continue;
        }

        private static QuestBoardInfo GetNearestQuestBoardInfo(RogueObj self)
        {
            var locationObjs = self.Location.Space.Objs;
            QuestBoardInfo nearestQuestBoardInfo = null;
            var nearestSqrDistance = int.MaxValue;
            for (int i = 0; i < locationObjs.Count; i++)
            {
                var obj = locationObjs[i];
                if (obj == null) continue;

                var info = QuestBoardInfo.Get(obj);
                if (info == null) continue;

                var sqrDistance = (obj.Position - self.Position).sqrMagnitude;
                if (sqrDistance < nearestSqrDistance)
                {
                    nearestQuestBoardInfo = info;
                    nearestSqrDistance = sqrDistance;
                }
            }
            return nearestQuestBoardInfo;
        }
    }
}
