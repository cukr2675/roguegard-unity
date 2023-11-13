using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CommandUnride : CommandRogueMethod
    {
        public override IKeyword Keyword => StdKw.Unride;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (CommonAssert.ObjDoesNotHaveToolAbility(self)) return false;

            var vehicle = RideRogueEffect.GetVehicle(self);
            if (vehicle == null) return false;

            var vehicleInfo = VehicleInfo.Get(vehicle);
            var result = RogueMethodAspectState.Invoke(
                StdKw.Unride, vehicleInfo.BeUnridden, vehicle, self, activationDepth, RogueMethodArgument.Identity);
            return result;
        }

        public override ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool)
        {
            var vehicle = RideRogueEffect.GetVehicle(self);
            if (vehicle == null) return null;

            var vehicleInfo = VehicleInfo.Get(vehicle);
            return vehicleInfo.BeUnridden;
        }
    }
}
