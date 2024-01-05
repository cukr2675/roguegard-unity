using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class WanderingBehaviourNode : IRogueBehaviourNode
    {
        private WanderingWalker walker = new WanderingWalker(RoguegardSettings.MaxTilemapSize);

        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            var nextPosition = walker.GetWalk(self, false);
            var direction = RogueDirection.FromSignOrLowerLeft(nextPosition - self.Position);
            default(IActiveRogueMethodCaller).Walk(self, direction, activationDepth);
            return RogueObjUpdaterContinueType.Break;
        }
    }
}
