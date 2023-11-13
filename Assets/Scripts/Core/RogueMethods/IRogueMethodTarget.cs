using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IRogueMethodTarget : IRogueDescription
    {
        IRoguePredicator GetPredicator(RogueObj self, float predictionDepth, RogueObj tool);
    }
}
