using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IStandardRaceOption : IRaceOption
    {
        int MinSize { get; }
        int MaxSize { get; }
        int TypeCount { get; }
        int MotionSetCount { get; }
    }
}
