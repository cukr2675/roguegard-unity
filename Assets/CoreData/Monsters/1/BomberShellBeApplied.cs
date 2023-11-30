using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class BomberShellBeApplied : BaseApplyRogueMethod
    {
        private BomberShellBeApplied() { }

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var movement = MovementCalculator.Get(self);
            var from = self.Position;
            var direction = RogueMethodUtility.GetTargetDirection(user, arg);
            LineOfSight10RogueMethodRange.Instance.Raycast(
                self.Location, from, direction, true, movement.HasTileCollider, out var hitObj, out var hitPosition, out var to);
            if (hitObj != null) { to -= direction.Forward; }
            MainCharacterWorkUtility.TryAddBeShot(self, user, hitPosition, from, CoreMotions.BeThrownFlying);

            // 移動前に生成する
            var syncWork = RogueCharacterWork.CreateSync(self);

            if (SpaceUtility.TryLocate(self, to))
            {
                if (RogueDevice.Primary.VisibleAt(self.Location, self.Position))
                {
                    RogueDevice.AddWork(DeviceKw.EnqueueWork, syncWork);
                    var work = RogueCharacterWork.CreateWalk(self, self.Position, self.Main.Stats.Direction, KeywordBoneMotion.Walk, true);
                    RogueDevice.AddWork(DeviceKw.EnqueueWork, work);
                }

                // 着弾地点で自爆する
                this.Defeat(self, user, activationDepth);
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
