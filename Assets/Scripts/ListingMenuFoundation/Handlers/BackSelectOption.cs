using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public class BackSelectOption : ISelectOption
    {
        private string name;
        private string style;

        public static BackSelectOption Instance { get; } = new BackSelectOption();

        public static BackSelectOption Create<TMgr, TArg>(string name = null, string style = null)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            var instance = new BackSelectOption();
            instance.name = name;
            instance.style = style;
            return instance;
        }

        string ISelectOption.GetName(IListMenuManager manager, IListMenuArg arg) => name ?? manager.BackOption.GetName(manager, arg);
        string ISelectOption.GetStyle(IListMenuManager manager, IListMenuArg arg) => style ?? manager.BackOption.GetStyle(manager, arg);
        void ISelectOption.HandleClick(IListMenuManager manager, IListMenuArg arg) => manager.BackOption.HandleClick(manager, arg);
    }
}
