using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public abstract class DirectionalBonePoseSourceData : ScriptableObject, IDirectionalBonePoseSource
    {
        public abstract BonePose GetBonePose(RogueDirection direction);
    }
}
