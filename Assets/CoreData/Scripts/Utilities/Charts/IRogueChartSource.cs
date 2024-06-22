using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.RequireRelationalComponent]
    public interface IRogueChartSource : System.IEquatable<IRogueChartSource>
    {
        IRogueChart CreateChart();
    }
}
