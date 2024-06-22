using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.RequireRelationalComponent]
    public interface IRogueChart
    {
        IRogueChartSource Source { get; }

        void MoveNext();
    }
}
