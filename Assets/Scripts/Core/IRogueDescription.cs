using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IRogueDescription
    {
        string Name { get; }
        Sprite Icon { get; }
        Color Color { get; }
        string Caption { get; }
        object Details { get; }
    }
}
