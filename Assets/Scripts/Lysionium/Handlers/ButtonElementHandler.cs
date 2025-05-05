using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    public class ButtonElementHandler<TElm, TMgr, TArg> : IButtonElementHandler
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public GetElementName<TElm, TMgr, TArg> GetName { get; set; }
        public GetElementStyle<TElm, TMgr, TArg> GetStyle { get; set; }
        public HandleClickElement<TElm, TMgr, TArg> HandleClick { get; set; }

        /// <summary>
        /// このインスタンスのデリゲート実行前に <see cref="SelectOptionHandler"/> の処理を挟む
        /// (リストの前後に <see cref="ISelectOption"/> を入れる場合を想定)
        /// </summary>
        public bool EnableSelectOptionProxy { get; set; }

        public ButtonElementHandler(bool enableSelectOptionProxy = true)
        {
            EnableSelectOptionProxy = enableSelectOptionProxy;
        }

        string IElementHandler.GetName(object element, IListMenuManager manager, IListMenuArg arg)
        {
            if (EnableSelectOptionProxy && element is ISelectOption) { return SelectOptionHandler.Instance.GetName(element, manager, arg); }

            if (LuiAssert.Type<TElm>(element, out var tElm) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                LuiAssert.Type<TArg>(arg, out var tArg)) return manager.ErrorOption.GetName(manager, arg);

            if (GetName != null) return GetName(tElm, tMgr, tArg);
            else return element?.ToString() ?? "null";
        }

        string IElementHandler.GetStyle(object element, IListMenuManager manager, IListMenuArg arg)
        {
            if (EnableSelectOptionProxy && element is ISelectOption) { return SelectOptionHandler.Instance.GetStyle(element, manager, arg); }

            if (LuiAssert.Type<TElm>(element, out var tElm) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                LuiAssert.Type<TArg>(arg, out var tArg)) return manager.ErrorOption.GetStyle(manager, arg);

            return GetStyle?.Invoke(tElm, tMgr, tArg);
        }

        void IButtonElementHandler.HandleClick(object element, IListMenuManager manager, IListMenuArg arg)
        {
            if (EnableSelectOptionProxy && element is ISelectOption)
            {
                SelectOptionHandler.Instance.HandleClick(element, manager, arg);
                return;
            }

            if (HandleClick == null) throw new System.InvalidOperationException($"{HandleClick} が null です。");
            if (LuiAssert.Type<TElm>(element, out var tElm, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

            HandleClick(tElm, tMgr, tArg);
        }
    }
}
