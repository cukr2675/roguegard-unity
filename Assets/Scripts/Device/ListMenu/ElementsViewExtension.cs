using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    [System.Obsolete]
    public static class ElementsViewExtension
    {
        public static void OpenView(
            this IElementsView view, IElementPresenter presenter, Spanning<object> list,
            object manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            view.OpenView(presenter, list, manager, self, user, arg);
        }
    }
}
