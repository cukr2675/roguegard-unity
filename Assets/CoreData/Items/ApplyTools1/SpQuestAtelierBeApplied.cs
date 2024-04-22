using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class SpQuestAtelierBeApplied : BaseApplyRogueMethod
    {
        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            RogueDevice.Add(DeviceKw.EnqueueSE, CategoryKw.DownStairs);
            this.Locate(user, null, self, new Vector2Int(2, 1), activationDepth);
            return false;
        }
    }
}
