using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class PushObstacleBehaviourNode : IRogueBehaviourNode
    {
        private static readonly PushCommand pushCommand = new PushCommand();

        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            foreach (var obj in self.Location.Space.Objs)
            {
                if (obj == null) continue;
                
                if ((self.Position - obj.Position).sqrMagnitude <= 2 && obj.Main.InfoSet.Category == CategoryKw.MovableObstacle)
                {
                    // 押せるものと隣接しているとき、それを押して移動させる
                    pushCommand.CommandInvoke(self, null, activationDepth, new(targetObj: obj));
                    return RogueObjUpdaterContinueType.Break;
                }
            }
            return RogueObjUpdaterContinueType.Continue;
        }
    }
}
