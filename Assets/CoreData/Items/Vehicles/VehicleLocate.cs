using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class VehicleLocate : ReferableScript, IChangeStateRogueMethod
    {
        private VehicleLocate() { }

        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (SpaceUtility.ObjIsGlued(self)) return false;

            var selfEquipmentInfo = self.Main.GetEquipmentInfo(self);
            if (selfEquipmentInfo != null && selfEquipmentInfo.EquipIndex >= 0)
            {
                // 装備されている装備品を空間移動させる場合、取り外してから移動させる。
                // 取り外せないときは移動を失敗させる。
                var unequipResult = this.TryUnequip(self, user, activationDepth);
                if (!unequipResult) return false;
            }

            var vehicleInfo = VehicleInfo.Get(self);
            if (vehicleInfo != null && vehicleInfo.Rider != null && activationDepth <= 0)
            {
                // 乗り物を空間移動させる場合、降ろしてから移動させる。
                // 降ろせないときは移動を失敗させる。
                var unrideResult = RogueMethodAspectState.Invoke(
                    StdKw.Unride, vehicleInfo.BeUnridden, self, user, 1, RogueMethodArgument.Identity);
                if (!unrideResult) return false;
            }

            var targetLocation = arg.TargetObj;
            if (targetLocation != null && targetLocation == RogueDevice.Primary?.Player?.Location)
            {
                // 空間移動時は自動表示を遅らせる。
                RogueDevice.Add(DeviceKw.InsertHideCharacterWork, self);
            }

            if (arg.TryGetTargetPosition(out var targetPosition))
            {
                var locateResult = SpaceUtility.TryLocate(self, targetLocation, targetPosition);
                return locateResult;
            }
            else
            {
                var locateResult = SpaceUtility.TryLocate(self, targetLocation);
                return locateResult;
            }
        }
    }
}
