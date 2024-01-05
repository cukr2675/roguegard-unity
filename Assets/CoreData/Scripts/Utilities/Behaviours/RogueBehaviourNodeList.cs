using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class RogueBehaviourNodeList //: IReadOnlyList<IRogueBehaviourNode>
    {
        private readonly List<IRogueBehaviourNode> nodes = new List<IRogueBehaviourNode>();

        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            foreach (var node in nodes)
            {
                var result = node.Tick(self, activationDepth);
                if (result == RogueObjUpdaterContinueType.Break) return RogueObjUpdaterContinueType.Break;
            }
            return RogueObjUpdaterContinueType.Continue;
        }

        public void Add(IRogueBehaviourNode node)
        {
            nodes.Add(node);
        }
    }
}
