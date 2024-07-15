using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public abstract class RogueMaterial : RogueDescriptionData, IRogueMaterial
    {
        public abstract void AffectValue(EffectableValue value, RogueObj self);
    }
}
