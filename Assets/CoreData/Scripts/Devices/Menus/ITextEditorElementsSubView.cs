using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;

namespace Roguegard.Device
{
    public interface ITextEditorElementsSubView : IElementsSubView
    {
        string Text { get; set; }
    }
}
