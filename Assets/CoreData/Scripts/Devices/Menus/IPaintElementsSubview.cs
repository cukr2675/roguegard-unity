using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RuntimeDotter;
using Lysionium;

namespace Roguegard.Device
{
    public interface IPaintElementsSubview : IElementsSubview
    {
        Spanning<DotterBoard> Boards { get; }
        Color32 MainColor { get; }
        Spanning<ShiftableColor> Palette { get; }

        void SetPaint(IReadOnlyList<DotterBoard> dotterBoards, Spanning<ShiftableColor> palette, Color32 mainColor, bool showSplitLine, Vector2[] pivots);
    }
}
