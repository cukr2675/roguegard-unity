using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IRogueMaterial : IRogueDescription
    {
        void AffectValue(EffectableValue value, RogueObj self);
    }
}
