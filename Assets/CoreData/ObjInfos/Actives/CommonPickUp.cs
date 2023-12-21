using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class CommonPickUp : ReferableScript, IActiveRogueMethod
    {
        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (CommonAssert.RequireTool(arg, out var tool)) return false;

            var weightCalculator = WeightCalculator.Get(self);
            var toolWeight = WeightCalculator.Get(tool).TotalWeight;
            var loadCapacity = StatsEffectedValues.GetLoadCapacity(self);
            if ((weightCalculator.SpaceWeight + toolWeight) > loadCapacity)
            {
                if (RogueDevice.Primary.Player == self)
                {
                    RogueDevice.Add(DeviceKw.AppendText, "これ以上持てない！\n");
                }
                return false;
            }

            if (!tool.AsTile &&             // 固定されているオブジェクトは拾えない。
                self.Location == tool.Location &&   // 別空間のオブジェクトは拾えない。
                self.Position == tool.Position &&   // 重なっていないオブジェクトは拾えない。
                this.Locate(tool, self, self, activationDepth)) // 拾う
            {
                if (MainCharacterWorkUtility.VisibleAt(self.Location, self.Position))
                {
                    RogueDevice.AddWork(DeviceKw.EnqueueWork, RogueCharacterWork.CreateSync(self));
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, "は");
                    RogueDevice.Add(DeviceKw.AppendText, tool);
                    RogueDevice.Add(DeviceKw.AppendText, "を拾った\n");
                    RogueDevice.Add(DeviceKw.EnqueueSE, MainInfoKw.PickUp);
                }
                return true;
            }
            else
            {
                if (RogueDevice.Primary.Player == self)
                {
                    RogueDevice.Add(DeviceKw.AppendText, tool);
                    RogueDevice.Add(DeviceKw.AppendText, "を拾えなかった\n");
                }
                return false;
            }
        }
    }
}
