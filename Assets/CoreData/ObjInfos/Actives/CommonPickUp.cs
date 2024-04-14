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
                if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
                {
                    using var handler = h;
                    handler.EnqueueWork(RogueCharacterWork.CreateSync(self));
                    handler.AppendText(self).AppendText("は").AppendText(tool);
                    if (tool.Stack >= 2)
                    {
                        handler.AppendText("x").AppendText(tool.Stack);
                    }
                    handler.AppendText("を拾った\n");
                    handler.EnqueueSE(MainInfoKw.PickUp);
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
