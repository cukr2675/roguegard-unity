using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IClosableStatusEffect : IStatusEffect
    {
        void Close(RogueObj self);
    }
}
