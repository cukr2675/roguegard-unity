using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

namespace ListingMF
{
    public static class SelectOption
    {
        public static SelectOption<TMgr, TArg> Create<TMgr, TArg>(
            string name, HandleClickElement<TMgr, TArg> handleClick, string style = null)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            var instance = new SelectOption<TMgr, TArg>();
            instance.SetName(name);
            instance.Style = style;
            instance.HandleClick = handleClick;
            return instance;
        }

        public static SelectOption<TMgr, TArg> Create<TMgr, TArg>(
            GetElementName<TMgr, TArg> getName, HandleClickElement<TMgr, TArg> handleClick, string style = null)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            var instance = new SelectOption<TMgr, TArg>();
            instance.SetName(getName);
            instance.Style = style;
            instance.HandleClick = handleClick;
            return instance;
        }
    }

    public class SelectOption<TMgr, TArg> : ISelectOption
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private string name;
        private GetElementName<TMgr, TArg> getName;

        public string Style { get; set; }

        public HandleClickElement<TMgr, TArg> HandleClick { get; set; }

        public void SetName(string name)
        {
            if (name == null) throw new System.ArgumentNullException(nameof(name));

            this.name = name;
            getName = null;
        }

        public void SetName(GetElementName<TMgr, TArg> getName)
        {
            if (getName == null) throw new System.ArgumentNullException(nameof(getName));

            this.getName = getName;
            name = null;
        }

        string ISelectOption.GetName(IListMenuManager manager, IListMenuArg arg)
        {
            if (getName != null)
            {
                if (LMFAssert.Type<TMgr>(manager, out var tMgr) ||
                    LMFAssert.Type<TArg>(arg, out var tArg)) return null;

                return getName(tMgr, tArg);
            }
            else return name;
        }

        string ISelectOption.GetStyle(IListMenuManager manager, IListMenuArg arg)
        {
            return Style;
        }

        void ISelectOption.HandleClick(IListMenuManager manager, IListMenuArg arg)
        {
            if (LMFAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                LMFAssert.Type<TArg>(arg, out var tArg, manager)) return;

            HandleClick(tMgr, tArg);
        }
    }
}
