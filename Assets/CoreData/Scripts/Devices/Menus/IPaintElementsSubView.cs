using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RuntimeDotter;
using ListingMF;

namespace Roguegard.Device
{
    public interface IPaintElementsSubView : IElementsSubView
    {
        Spanning<DotterBoard> Boards { get; }
        Color32 MainColor { get; }
        Spanning<ShiftableColor> Palette { get; }

        void SetPaint(IReadOnlyList<DotterBoard> dotterBoards, object other, bool showSplitLine, Vector2[] pivots);
    }
}
