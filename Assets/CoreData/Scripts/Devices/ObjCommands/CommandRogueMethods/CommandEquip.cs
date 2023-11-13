using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class CommandEquip : CommandRogueMethod
    {
        public override string Name => MainInfoKw.Equip.Name;

        public override IKeyword Keyword => MainInfoKw.Equip;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (CommonAssert.RequireTool(arg, out var tool)) return false;
            if (CommonAssert.ObjDoesNotHaveToolAbility(self)) return false;

            if (tool.Location != self)
            {
                if (RogueDevice.Primary.Player == self)
                {
                    RogueDevice.Add(DeviceKw.AppendText, tool);
                    RogueDevice.Add(DeviceKw.AppendText, "は装備できない場所にある\n");
                }
                return false;
            }

            var result = this.TryEquip(tool, self, activationDepth);
            return result;
        }

        public override bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (CommonAssert.RequireTool(arg, out var tool)) return false;
            if (CommonAssert.ObjDoesNotHaveToolAbility(self)) return false;

            EnqueueMessageRule(Keyword);

            if (tool.Location == self.Location)
            {
                // 同一空間にあるときは拾ってから装備する。
                var pickUpMethod = self.Main.InfoSet.PickUp;
                RogueMethodAspectState.Invoke(MainInfoKw.PickUp, pickUpMethod, self, user, activationDepth, arg);
                if (tool.Location != self) return false;
            }

            return RogueMethodAspectState.Invoke(Keyword, this, self, user, activationDepth, arg);
        }

        public override ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool)
        {
            var equipmentInfo = tool?.Main.GetEquipmentInfo(tool);
            return equipmentInfo?.BeEquipped;
        }
    }
}
