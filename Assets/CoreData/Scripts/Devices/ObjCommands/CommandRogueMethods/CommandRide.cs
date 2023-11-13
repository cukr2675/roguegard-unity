using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CommandRide : CommandRogueMethod
    {
        public override IKeyword Keyword => StdKw.Ride;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (CommonAssert.ObjDoesNotHaveToolAbility(self)) return false;
            if (CommonAssert.RequireTool(arg, out var vehicle)) return false;

            var oldVehicle = RideRogueEffect.GetVehicle(self);
            if (oldVehicle != null)
            {
                // すでに乗り物に乗っていたら降りる。
                var oldVehicleInfo = VehicleInfo.Get(oldVehicle);
                var unrideResult = RogueMethodAspectState.Invoke(
                    StdKw.Unride, oldVehicleInfo.BeUnridden, oldVehicle, self, activationDepth, RogueMethodArgument.Identity);
                if (!unrideResult) return false;
            }

            var vehicleInfo = VehicleInfo.Get(vehicle);
            if (vehicleInfo == null) return false;

            var result = RogueMethodAspectState.Invoke(
                StdKw.Ride, vehicleInfo.BeRidden, vehicle, self, activationDepth, RogueMethodArgument.Identity);
            return result;
        }

        public override ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool)
        {
            if (tool == null) return null;

            var vehicleInfo = VehicleInfo.Get(tool);
            return vehicleInfo?.BeRidden;
        }
    }
}
