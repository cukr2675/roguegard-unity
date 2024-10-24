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
        public HandleClickElement<TElm, TMgr, TArg> HandleClick { get; set; }

        //public delegate string GetNameFunc(TElm element, TMgr manager, TArg arg);
        //public delegate void HandleClickAction(TElm element, TMgr manager, TArg arg);

        string IElementHandler.GetName(object element, IListMenuManager manager, IListMenuArg arg)
        {
            if (LMFAssert.Type<TElm>(element, out var tElm) ||
                LMFAssert.Type<TMgr>(manager, out var tMgr) ||
                LMFAssert.Type<TArg>(arg, out var tArg)) return string.Empty;

            if (GetName != null) return GetName(tElm, tMgr, tArg);
            else return element?.ToString() ?? "null";
        }

        void IButtonElementHandler.HandleClick(object element, IListMenuManager manager, IListMenuArg arg)
        {
            if (LMFAssert.Type<TElm>(element, out var tElm, manager) ||
                LMFAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                LMFAssert.Type<TArg>(arg, out var tArg, manager)) return;

            HandleClick(tElm, tMgr, tArg);
        }
    }
}
