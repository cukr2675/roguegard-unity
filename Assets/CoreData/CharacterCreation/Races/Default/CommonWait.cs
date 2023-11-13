using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CommonWait : ReferableScript, IActiveRogueMethod
    {
        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (RogueDevice.Primary.VisibleAt(self.Location, self.Position))
            {
                // 1 ターンずつ足踏みモーションを待機する場合
                //var item = RogueCharacterWork.CreateWalk(self, self.Position, -1f, self.Main.Stats.Direction, KeywordBoneMotion.Walk, true);

                var item = RogueCharacterWork.CreateWalk(self, self.Position, self.Main.Stats.Direction, KeywordBoneMotion.Walk, true);
                RogueDevice.AddWork(DeviceKw.EnqueueWork, item);
            }
            return true;
        }
    }
}
