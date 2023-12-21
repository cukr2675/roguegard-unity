using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class AmuletOfChangeBeEquipped : BaseApplyRogueMethod
    {
        private AmuletOfChangeBeEquipped() { }

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var info = self.Main.GetEquipmentInfo(self);
            if (!info.CanStackWhileEquipped && self.Stack >= 2)
            {
                // 装備中スタック不可の装備品がスタックしていたら一つだけ装備する。
                if (!SpaceUtility.TryDividedLocate(self, 1, out self)) return false;

                info = self.Main.GetEquipmentInfo(self);
            }

            var equipResult = info.TryOpen(self, arg.Count);
            if (!equipResult) return false;

            // 装備に成功したら装備を破壊してステータスエフェクトを付与する。
            var owner = self.Location;
            info.RemoveClose(self);
            self.TrySetStack(0, owner);

            ChangeGenderEffect.Change(owner);
            if (MainCharacterWorkUtility.VisibleAt(owner.Location, owner.Position))
            {
                RogueDevice.Add(DeviceKw.AppendText, owner);
                RogueDevice.Add(DeviceKw.AppendText, "は性別が変わった\n");
            }
            return true;
        }
    }
}
