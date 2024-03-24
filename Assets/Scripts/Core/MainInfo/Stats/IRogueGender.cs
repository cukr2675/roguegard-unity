using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.RequireRelationalComponent]
    public interface IRogueGender : IRogueDescription
    {
        void AffectValue(AffectableValue value, RogueObj self, MainInfoSetType infoSetType);
    }
}
