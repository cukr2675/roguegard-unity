using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class CommonLocate : ReferableScript, IChangeStateRogueMethod
    {
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

            var targetLocation = arg.TargetObj;
            bool locateResult;
            if (arg.TryGetTargetPosition(out var targetPosition))
            {
                locateResult = SpaceUtility.TryLocate(self, targetLocation, targetPosition);
            }
            else
            {
                locateResult = SpaceUtility.TryLocate(self, targetLocation);
            }

            if (locateResult && targetLocation != null && targetLocation == RogueDevice.Primary?.Player?.Location && self.Stack != 0)
            {
                // 空間移動時は自動表示を遅らせる。移動先でスタックした場合は何もしない
                RogueDevice.Add(DeviceKw.InsertHideCharacterWork, self);
            }
            return locateResult;
        }
    }
}
