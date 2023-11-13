using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class WalkCommandAction : IDeviceCommandAction
    {
        public static WalkCommandAction Instance { get; } = new WalkCommandAction();

        public bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var method = self.Main.InfoSet.Walk;
            return RogueMethodAspectState.Invoke(MainInfoKw.Walk, method, self, user, activationDepth, arg);
        }
    }
}
