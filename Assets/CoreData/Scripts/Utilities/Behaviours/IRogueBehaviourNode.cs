using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IRogueBehaviourNode
    {
        RogueObjUpdaterContinueType Tick(RogueObj self, float activationDepth);
    }
}
