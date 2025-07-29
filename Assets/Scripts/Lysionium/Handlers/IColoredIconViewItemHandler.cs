using UnityEngine;

namespace Lysionium
{
    internal interface IColoredIconViewItemHandler : IViewItemHandler
    {
        void GetIcon(object item, IListMenuManager manager, IListMenuArg arg, out Sprite sprite, out Color color);
    }
}
