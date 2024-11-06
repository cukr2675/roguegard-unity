using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public class ButtonElementHandler<TElm, TMgr, TArg> : IButtonElementHandler
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public GetElementName<TElm, TMgr, TArg> GetName { get; set; }
        public GetElementIcon<TElm, TMgr, TArg> GetIcon { get; set; }
        public GetElementStyle<TElm, TMgr, TArg> GetStyle { get; set; }
        public HandleClickElement<TElm, TMgr, TArg> HandleClick { get; set; }

        string IElementHandler.GetName(object element, IListMenuManager manager, IListMenuArg arg)
        {
            if (LMFAssert.Type<TElm>(element, out var tElm) ||
                LMFAssert.Type<TMgr>(manager, out var tMgr) ||
                LMFAssert.Type<TArg>(arg, out var tArg)) return manager.ErrorOption.GetName(manager, arg);

            if (GetName != null) return GetName(tElm, tMgr, tArg);
            else return element?.ToString() ?? "null";
        }

        Sprite IElementHandler.GetIcon(object element, IListMenuManager manager, IListMenuArg arg)
        {
            if (LMFAssert.Type<TElm>(element, out var tElm) ||
                LMFAssert.Type<TMgr>(manager, out var tMgr) ||
                LMFAssert.Type<TArg>(arg, out var tArg)) return null;

            return GetIcon?.Invoke(tElm, tMgr, tArg);
        }

        string IElementHandler.GetStyle(object element, IListMenuManager manager, IListMenuArg arg)
        {
            if (LMFAssert.Type<TElm>(element, out var tElm) ||
                LMFAssert.Type<TMgr>(manager, out var tMgr) ||
                LMFAssert.Type<TArg>(arg, out var tArg)) return manager.ErrorOption.GetStyle(manager, arg);

            return GetStyle?.Invoke(tElm, tMgr, tArg);
        }

        void IButtonElementHandler.HandleClick(object element, IListMenuManager manager, IListMenuArg arg)
        {
            if (HandleClick == null) throw new System.InvalidOperationException($"{HandleClick} Ç™ null Ç≈Ç∑ÅB");
            if (LMFAssert.Type<TElm>(element, out var tElm, manager) ||
                LMFAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                LMFAssert.Type<TArg>(arg, out var tArg, manager)) return;

            HandleClick(tElm, tMgr, tArg);
        }
    }
}
