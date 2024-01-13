using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class IfInLobbyBehaviourNode : IRogueBehaviourNode
    {
        public RogueBehaviourNodeList InLobbyNode { get; } = new RogueBehaviourNodeList();
        public RogueBehaviourNodeList OtherNode { get; } = new RogueBehaviourNodeList();

        public RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth)
        {
            var worldInfo = RogueWorldInfo.GetByCharacter(self);
            if (self.Location == worldInfo.Lobby)
            {
                return InLobbyNode.Tick(self, activationDepth);
            }
            else
            {
                return OtherNode.Tick(self, activationDepth);
            }
        }
    }
}
