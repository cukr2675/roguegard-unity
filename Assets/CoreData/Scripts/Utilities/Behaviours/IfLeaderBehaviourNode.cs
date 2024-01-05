using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class IfLeaderBehaviourNode : IRogueBehaviourNode
    {
        public RogueBehaviourNodeList LeaderNode { get; set; }
        public RogueBehaviourNodeList OtherNode { get; set; }

        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            if (self.Main.GetPlayerLeaderInfo(self) != null)
            {
                return LeaderNode.Tick(self, activationDepth);
            }
            else
            {
                return OtherNode.Tick(self, activationDepth);
            }
        }
    }
}
