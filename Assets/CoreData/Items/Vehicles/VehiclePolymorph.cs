using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class VehiclePolymorph : ReferableScript, IChangeStateRogueMethod
    {
        private VehiclePolymorph() { }

        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (!(arg.Other is MainInfoSet infoSet))
            {
                Debug.LogError("引数が不正です。");
                return false;
            }

            var selfEquipmentInfo = self.Main.GetEquipmentInfo(self);
            var vehicleInfo = VehicleInfo.Get(self);
            if (selfEquipmentInfo != null)
            {
                // 装備品自体を変化させるとき（装備者を変化させるわけではない）
                var equipIndex = selfEquipmentInfo.EquipIndex;
                if (equipIndex >= 0)
                {
                    // 装備品を変化させる場合、装備を解除してから変化させる。
                    selfEquipmentInfo.RemoveClose(self);
                }

                self.Main.Polymorph(self, infoSet);

                var polymorphedEquipmentInfo = self.Main.GetEquipmentInfo(self);
                if (equipIndex >= 0 && polymorphedEquipmentInfo != null)
                {
                    // 変化前に装備されていた場合、変化後の装備品を装備する。
                    polymorphedEquipmentInfo.TryOpen(self, equipIndex);
                }
            }
            else if (vehicleInfo != null)
            {
                // 乗り物自体を変化させるとき
                var rider = vehicleInfo.Rider;
                if (rider != null)
                {
                    vehicleInfo.RemoveClose(self);
                }

                self.Main.Polymorph(self, infoSet);

                var polymorphedVehicleInfo = VehicleInfo.Get(self);
                if (rider != null && polymorphedVehicleInfo != null)
                {
                    polymorphedVehicleInfo.TryOpen(self, rider);
                }
            }
            else
            {
                // 装備品以外
                self.Main.Polymorph(self, infoSet);
            }
            return true;
        }
    }
}
