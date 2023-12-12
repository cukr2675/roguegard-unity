using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class WandOfPinningBeApplied : ConsumeApplyRogueMethod
    {
        private WandOfPinningBeApplied() { }

        public override IRogueMethodTarget Target => null;
        public override IRogueMethodRange Range => null;

        protected override bool BeApplied(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            //var positioning = RogueCharacterWork.CreateSyncPositioning(user);
            //RogueDevice.AddWork(DeviceKw.EnqueueWork, user, positioning);
            //var velocity = Vector2Utility.GetNormalByDegree(user.Main.Stats.Degree);
            //var result = SpaceUtility.Shot(
            //    user, user, activationDepth, user.Location, user.Position, velocity, 10, out var hitObj);
            //if (result == SpaceUtility.ShotResult.Miss)
            //{
            //    RogueDevice.Add(DeviceKw.Soliloquy, user, "移動は失敗した！\n");
            //    return false;
            //}

            //var item = RogueCharacterWork.CreateWalk(user, user.Position, 1f, null, false);
            //RogueDevice.AddWork(DeviceKw.EnqueueWork, user, item);
            return true;
        }
    }
}
