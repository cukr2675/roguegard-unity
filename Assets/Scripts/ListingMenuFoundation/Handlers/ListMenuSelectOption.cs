using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public static class ListMenuSelectOption
    {
        public static ListMenuSelectOption<TMgr, TArg> Create<TMgr, TArg>(
            string name, ListMenuSelectOption<TMgr, TArg>.HandleClickAction handleClick)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            return new ListMenuSelectOption<TMgr, TArg>()
            {
                Name = name,
                HandleClick = handleClick
            };
        }
    }

    public class ListMenuSelectOption<TMgr, TArg> : IListMenuSelectOption
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public string Name { get; set; }
        public HandleClickAction HandleClick { get; set; }

        public delegate void HandleClickAction(TMgr manager, TArg arg);

        string IListMenuSelectOption.GetName(IListMenuManager manager, IListMenuArg arg) => Name;

        void IListMenuSelectOption.HandleClick(IListMenuManager manager, IListMenuArg arg)
        {
            if (LMFAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                LMFAssert.Type<TArg>(arg, out var tArg, manager)) return;

            HandleClick(tMgr, tArg);
        }
    }
}
