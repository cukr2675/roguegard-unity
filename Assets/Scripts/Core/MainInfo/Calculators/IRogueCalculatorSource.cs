using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IRogueCalculatorSource
    {
        IRogueCalculator Create();
    }
}
