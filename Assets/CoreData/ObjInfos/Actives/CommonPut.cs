using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class CommonPut : ReferableScript, IActiveRogueMethod
    {
        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var tool = arg.Tool;
            if (tool == null)
            {
                Debug.Log("使用する道具が見つかりません。");
                return false;
            }

            if (!tool.AsTile &&     // 固定されているオブジェクトは置けない。
                tool.Location == self &&    // 所持していないオブジェクトは置けない。
                this.Locate(tool, self, self.Location, self.Position, activationDepth)) // 置く
            {
                if (RogueDevice.Primary.VisibleAt(self.Location, self.Position))
                {
                    RogueDevice.AddWork(DeviceKw.EnqueueWork, RogueCharacterWork.CreateSync(self));
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, "は");
                    RogueDevice.Add(DeviceKw.AppendText, tool);
                    RogueDevice.Add(DeviceKw.AppendText, "を地面に置いた\n");
                }
                return true;
            }
            else
            {
                if (RogueDevice.Primary.Player == self)
                {
                    RogueDevice.Add(DeviceKw.AppendText, tool);
                    RogueDevice.Add(DeviceKw.AppendText, "を地面に置けなかった\n");
                }
                return false;
            }
        }
    }
}
