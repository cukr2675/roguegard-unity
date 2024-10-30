using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public static class ListMenuSelectOption
    {
        public static ListMenuSelectOption<TMgr, TArg> Create<TMgr, TArg>(string name, HandleClickElement<TMgr, TArg> handleClick)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            var instance = new ListMenuSelectOption<TMgr, TArg>();
            instance.SetName(name);
            instance.HandleClick = handleClick;
            return instance;
        }

        public static ListMenuSelectOption<TMgr, TArg> Create<TMgr, TArg>(GetElementName<TMgr, TArg> getName, HandleClickElement<TMgr, TArg> handleClick)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            var instance = new ListMenuSelectOption<TMgr, TArg>();
            instance.SetName(getName);
            instance.HandleClick = handleClick;
            return instance;
        }
    }

    public class ListMenuSelectOption<TMgr, TArg> : IListMenuSelectOption
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private string name;
        private GetElementName<TMgr, TArg> getName;

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

        string IListMenuSelectOption.GetName(IListMenuManager manager, IListMenuArg arg)
        {
            if (getName != null)
            {
                if (LMFAssert.Type<TMgr>(manager, out var tMgr) ||
                    LMFAssert.Type<TArg>(arg, out var tArg)) return null;

                return getName(tMgr, tArg);
            }
            else return name;
        }

        void IListMenuSelectOption.HandleClick(IListMenuManager manager, IListMenuArg arg)
        {
            if (LMFAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                LMFAssert.Type<TArg>(arg, out var tArg, manager)) return;

            HandleClick(tMgr, tArg);
        }
    }
}
