using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    /// <summary>
    /// モデルのリストではなく選択肢を扱いたいときに使用する <see cref="IElementPresenter"/> 。
    /// </summary>
    public class SelectOptionPresenter : IElementPresenter
    {
        public static SelectOptionPresenter Instance { get; } = new SelectOptionPresenter();

        public string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var selectOption = (IListMenuSelectOption)element;
            return selectOption.GetName(manager, self, user, arg);
        }

        public void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var selectOption = (IListMenuSelectOption)element;
            selectOption.Activate(manager, self, user, arg);
        }
    }
}
