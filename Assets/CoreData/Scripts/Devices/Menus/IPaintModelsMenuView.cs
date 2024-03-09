using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RuntimeDotter;

namespace Roguegard
{
    public interface IPaintModelsMenuView : IScrollModelsMenuView
    {
        Spanning<DotterBoard> Boards { get; }
        Color32 MainColor { get; }
        Spanning<ShiftableColor> Palette { get; }

        void ShowSplitLine(bool showSplitLine, Spanning<Vector2> pivots);
    }
}
