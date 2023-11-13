using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class WaitCommandAction : IDeviceCommandAction
    {
        public static WaitCommandAction Instance { get; } = new WaitCommandAction();

        public bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var method = self.Main.InfoSet.Wait;
            return RogueMethodAspectState.Invoke(MainInfoKw.Wait, method, self, user, activationDepth, arg);
        }
    }
}
