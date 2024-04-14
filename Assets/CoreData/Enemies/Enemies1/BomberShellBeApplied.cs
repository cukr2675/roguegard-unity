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

            if (self.Position == to || SpaceUtility.TryLocate(self, to))
            {
                if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
                {
                    using var handler = h;
                    handler.EnqueueWork(syncWork);
                    handler.EnqueueWork(RogueCharacterWork.CreateWalk(self, self.Position, self.Main.Stats.Direction, KeywordSpriteMotion.Walk, true));
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
