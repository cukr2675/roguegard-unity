using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    internal interface IColoredIconElementHandler : IElementHandler
    {
        void GetIcon(object element, IListMenuManager manager, IListMenuArg arg, out Sprite sprite, out Color color);
    }
}
