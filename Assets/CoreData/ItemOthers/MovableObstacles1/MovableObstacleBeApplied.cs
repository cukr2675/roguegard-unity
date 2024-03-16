using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class MovableObstacleBeApplied : BaseApplyRogueMethod
    {
        private MovableObstacleBeApplied() { }

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (!arg.TryGetTargetPosition(out var targetPosition)) return false;

            // 移動前に生成する
            var syncWork = RogueCharacterWork.CreateSync(self);

            if (SpaceUtility.TryLocate(self, targetPosition))
            {
                if (MainCharacterWorkUtility.VisibleAt(self.Location, self.Position))
                {
                    RogueDevice.AddWork(DeviceKw.EnqueueWork, syncWork);
                    var work = RogueCharacterWork.CreateWalk(self, self.Position, self.Main.Stats.Direction, KeywordSpriteMotion.Walk, true);
                    RogueDevice.AddWork(DeviceKw.EnqueueWork, work);
                }
                return true;
            }
            else
            {
                //RogueDevice.Add(DeviceKw.Soliloquy, self, "押したがびくともしなかった");
                return false;
            }
        }
    }
}
