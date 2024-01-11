using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class AcceptQuestBehaviourNode : IRogueBehaviourNode
    {
        private int count;

        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            if (self.Location == null) return RogueObjUpdaterContinueType.Continue;

            count++;
            if (count >= 10)
            {
                // 10ターン後にクエストへ出発
                var spaceObjs = self.Location.Space.Objs;
                for (int i = 0; i < spaceObjs.Count; i++)
                {
                    var obj = spaceObjs[i];
                    if (obj == null) continue;

                    var quests = QuestBoardInfo.GetQuestList(obj);
                    if (quests == null || quests.Count == 0) continue;

                    count = 0;
                    var quest = quests[0];
                    quests.RemoveAt(0);
                    quest.Start(self);
                    return RogueObjUpdaterContinueType.Break;
                }
            }

            return RogueObjUpdaterContinueType.Continue;
        }
    }
}
