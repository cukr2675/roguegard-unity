using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lysionium;

namespace Roguegard.Device
{
    public interface ITextEditorElementsSubView : IElementsSubView
    {
        string Text { get; set; }
    }
}
