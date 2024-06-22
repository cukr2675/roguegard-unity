using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public static class ElementsViewExtension
    {
        public static void OpenView(
            this IElementsView view, IElementPresenter presenter, Spanning<object> list,
            IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            view.OpenView(presenter, list, manager, self, user, arg);
        }
    }
}
