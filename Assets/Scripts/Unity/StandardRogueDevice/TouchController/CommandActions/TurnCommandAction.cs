using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class TurnCommandAction : IDeviceCommandAction
    {
        public static TurnCommandAction Instance { get; } = new TurnCommandAction();

        private static TurnRogueMethod turnRogueMethod = new TurnRogueMethod();

        public bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            return turnRogueMethod.Invoke(self, user, activationDepth, arg);
            //return RogueMethodAspectState.Invoke(StdKw.Turn, turnRogueMethod, self, user, activationDepth, arg);
        }

        private class TurnRogueMethod : IActiveRogueMethod
        {
            public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                self.Main.Stats.Direction = RogueMethodUtility.GetTargetDirection(self, arg);
                MainCharacterWorkUtility.TryAddTurn(self);
                return false;
            }
        }
    }
}
