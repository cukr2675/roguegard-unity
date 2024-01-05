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
            var spaceObjs = self.Location.Space.Objs;
            for (int i = 0; i < spaceObjs.Count; i++)
            {
                var obj = spaceObjs[i];
                if (obj == null) continue;
                
                if ((self.Position - obj.Position).sqrMagnitude <= 2 && obj.Main.InfoSet.Category == CategoryKw.MovableObstacle)
                {
                    // ‰Ÿ‚¹‚é‚à‚Ì‚Æ—×Ú‚µ‚Ä‚¢‚é‚Æ‚«A‚»‚ê‚ð‰Ÿ‚µ‚ÄˆÚ“®‚³‚¹‚é
                    pushCommand.CommandInvoke(self, null, activationDepth, new(targetObj: obj));
                    return RogueObjUpdaterContinueType.Break;
                }
            }
            return RogueObjUpdaterContinueType.Continue;
        }
    }
}
