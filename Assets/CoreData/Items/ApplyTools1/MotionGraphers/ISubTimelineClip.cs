using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.RequireRelationalComponent]
    public interface ISubTimelineClip
    {
        float StartTime { get; }

        bool TryGet<T>(out T value);
    }
}
