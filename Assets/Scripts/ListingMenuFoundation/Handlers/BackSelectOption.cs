using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public class BackSelectOption : ISelectOption
    {
        public static BackSelectOption Instance { get; } = new BackSelectOption();

        public static BackSelectOption Create<TMgr, TArg>()
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            var instance = new BackSelectOption();
            return instance;
        }

        string ISelectOption.GetName(IListMenuManager manager, IListMenuArg arg) => manager.BackOption.GetName(manager, arg);
        string ISelectOption.GetStyle(IListMenuManager manager, IListMenuArg arg) => manager.BackOption.GetStyle(manager, arg);
        void ISelectOption.HandleClick(IListMenuManager manager, IListMenuArg arg) => manager.BackOption.HandleClick(manager, arg);
    }
}
