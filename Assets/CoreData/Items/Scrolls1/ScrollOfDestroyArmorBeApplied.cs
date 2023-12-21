using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class ScrollOfDestroyArmorBeApplied : ConsumeApplyRogueMethod
    {
        public override IRogueMethodTarget Target => ForEnemyRogueMethodTarget.Instance;
        public override IRogueMethodRange Range => UserRogueMethodRange.Instance;

        protected override bool BeApplied(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var visible = MainCharacterWorkUtility.VisibleAt(user.Location, user.Position);
            if (visible)
            {
                RogueDevice.Add(DeviceKw.AppendText, user);
                RogueDevice.Add(DeviceKw.AppendText, "は");
                RogueDevice.Add(DeviceKw.AppendText, self);
                RogueDevice.Add(DeviceKw.AppendText, "を読んだ！\n");
            }

            var equipment = EquipmentUtility.GetRandomArmor(user, RogueRandom.Primary);
            if (equipment != null)
            {
                equipment.TrySetStack(0, user);
                if (visible)
                {
                    RogueDevice.Add(DeviceKw.AppendText, user);
                    RogueDevice.Add(DeviceKw.AppendText, "が装備していた");
                    RogueDevice.Add(DeviceKw.AppendText, equipment);
                    RogueDevice.Add(DeviceKw.AppendText, "が砕け散った！\n");
                }
            }
            else
            {
                if (visible)
                {
                    RogueDevice.Add(DeviceKw.AppendText, "しかし何も起こらなかった！\n");
                }
            }
            return true;
        }
    }
}
