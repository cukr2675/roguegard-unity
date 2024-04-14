using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CommonWait : ReferableScript, IActiveRogueMethod
    {
        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
            {
                using var handler = h;

                // 1 ターンずつ足踏みモーションを待機する場合
                //var item = RogueCharacterWork.CreateWalk(self, self.Position, -1f, self.Main.Stats.Direction, KeywordSpriteMotion.Walk, true);

                var item = RogueCharacterWork.CreateWalk(self, self.Position, self.Main.Stats.Direction, KeywordSpriteMotion.Walk, true);
                handler.EnqueueWork(item);
            }
            return true;
        }
    }
}
