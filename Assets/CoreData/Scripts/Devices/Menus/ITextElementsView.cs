using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public interface ITextElementsView : IElementsView
    {
        string Text { get; }
    }
}
