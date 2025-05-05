using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lysionium;

namespace Roguegard.Device
{
    public interface ITextEditorElementsSubview : IElementsSubview
    {
        string Text { get; set; }
    }
}
