using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ListingMF
{
    /// <summary>
    /// モデルのリストではなく選択肢を扱いたいときに使用する <see cref="IElementHandler"/> 。
    /// </summary>
    public class SelectOptionHandler : IButtonElementHandler
    {
        public static SelectOptionHandler Instance { get; } = new SelectOptionHandler();

        public string GetName(object element, IListMenuManager manager, IListMenuArg arg)
        {
            if (LMFAssert.Type<IListMenuSelectOption>(element, out var selectOption)) return string.Empty;

            return selectOption.GetName(manager, arg);
        }

        public void HandleClick(object element, IListMenuManager manager, IListMenuArg arg)
        {
            if (LMFAssert.Type<IListMenuSelectOption>(element, out var selectOption, manager)) return;

            selectOption.HandleClick(manager, arg);
        }
    }
}
