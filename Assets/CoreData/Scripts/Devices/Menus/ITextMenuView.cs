using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public interface ITextMenuView : IModelListView
    {
        string Text { get; }
    }
}
