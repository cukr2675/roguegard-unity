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
            if (MovementUtility.TryGetApproachDirection(self, nextPosition, true, out var approachDirection))
            {
                default(IActiveRogueMethodCaller).Walk(self, approachDirection, activationDepth);
                walker.GetWalk(self, false); // �ړ���������̎��E�Ńp�X���X�V
            }
            return RogueObjUpdaterContinueType.Break;
        }
    }
}
