using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RuntimeDotter;
using Roguegard.Device;

namespace Roguegard
{
    public interface IPaintMenuView : IModelListView
    {
        Spanning<DotterBoard> Boards { get; }
        Color32 MainColor { get; }
        Spanning<ShiftableColor> Palette { get; }

        void ShowSplitLine(bool showSplitLine, Spanning<Vector2> pivots);
    }
}
