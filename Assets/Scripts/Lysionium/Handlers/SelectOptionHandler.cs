using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    /// <summary>
    /// モデルのリストではなく選択肢を扱いたいときに使用する <see cref="IElementHandler"/> 。
    /// </summary>
    public class SelectOptionHandler : IButtonElementHandler
    {
        public static SelectOptionHandler Instance { get; } = new SelectOptionHandler();

        public string GetName(object element, IListMenuManager manager, IListMenuArg arg)
        {
            if (LUIAssert.Type<ISelectOption>(element, out var selectOption)) return string.Empty;

            return selectOption.GetName(manager, arg);
        }

        public string GetStyle(object element, IListMenuManager manager, IListMenuArg arg)
        {
            if (LUIAssert.Type<ISelectOption>(element, out var selectOption)) return string.Empty;

            return selectOption.GetStyle(manager, arg);
        }

        public void HandleClick(object element, IListMenuManager manager, IListMenuArg arg)
        {
            if (LUIAssert.Type<ISelectOption>(element, out var selectOption, manager)) return;

            selectOption.HandleClick(manager, arg);
        }
    }
}
