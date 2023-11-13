using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public abstract class RogueGender : RogueDescriptionData, IRogueGender
    {
        public abstract void AffectValue(AffectableValue value, RogueObj self, MainInfoSetType infoSetType);
    }
}
