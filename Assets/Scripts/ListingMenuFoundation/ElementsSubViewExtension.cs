using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public static class ElementsSubViewExtension
    {
        public static void Show(
            this IElementsSubView subView, IReadOnlyList<object> list, IElementHandler handler,
            IListMenuManager manager, IListMenuArg arg, ref IElementsSubViewStateProvider stateProvider,
            ElementsSubView.HandleEndAnimationAction handleEndAnimation = null)
        {
            subView.SetParameters(list, handler, manager, arg, ref stateProvider);
            subView.Show(handleEndAnimation);
        }
    }
}
