using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    public static class ElementsSubviewExtension
    {
        public static void Show(
            this IElementsSubview subview, IReadOnlyList<object> list, IElementHandler handler,
            IListMenuManager manager, IListMenuArg arg, ref IElementsSubviewStateProvider stateProvider,
            HandleEndAnimation onEndAnimation = null)
        {
            subview.SetParameters(list, handler, manager, arg, ref stateProvider);
            subview.Show(onEndAnimation);
        }
    }
}
