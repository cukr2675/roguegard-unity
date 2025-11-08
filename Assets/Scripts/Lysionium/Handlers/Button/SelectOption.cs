namespace Lysionium
{
    public static class SelectOption
    {
        public static SelectOption<TMgr, TArg> Create<TMgr, TArg>(
            string name, ClickItemHandler<TMgr, TArg> onClick, string style = "")
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            var instance = new SelectOption<TMgr, TArg>();
            instance.SetName(name);
            instance.SetStyle(style);
            instance.Click = onClick;
            return instance;
        }

        public static SelectOption<TMgr, TArg> Create<TMgr, TArg>(
            System.Func<TMgr, TArg, string> getName, ClickItemHandler<TMgr, TArg> onClick, string style = "")
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            var instance = new SelectOption<TMgr, TArg>();
            instance.SetName(getName);
            instance.SetStyle(style);
            instance.Click = onClick;
            return instance;
        }

        public static SelectOption<TMgr, TArg> Create<TMgr, TArg>(
            string name, ClickItemHandler<TMgr, TArg> onClick, System.Func<TMgr, TArg, string> style)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            var instance = new SelectOption<TMgr, TArg>();
            instance.SetName(name);
            instance.SetStyle(style);
            instance.Click = onClick;
            return instance;
        }

        public static SelectOption<TMgr, TArg> Create<TMgr, TArg>(
            System.Func<TMgr, TArg, string> getName, ClickItemHandler<TMgr, TArg> onClick, System.Func<TMgr, TArg, string> style)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            var instance = new SelectOption<TMgr, TArg>();
            instance.SetName(getName);
            instance.SetStyle(style);
            instance.Click = onClick;
            return instance;
        }
    }

    public class SelectOption<TMgr, TArg> : ISelectOption
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private string name;
        private System.Func<TMgr, TArg, string> getName;

        private string style;
        private System.Func<TMgr, TArg, string> getStyle;

        public ClickItemHandler<TMgr, TArg> Click { get; set; }

        public void SetName(string name)
        {
            this.name = name ?? throw new System.ArgumentNullException(nameof(name));
            getName = null;
        }

        public void SetName(System.Func<TMgr, TArg, string> selector)
        {
            getName = selector ?? throw new System.ArgumentNullException(nameof(selector));
            name = null;
        }

        public void SetStyle(string style)
        {
            this.style = style;
            getStyle = null;
        }

        public void SetStyle(System.Func<TMgr, TArg, string> selector)
        {
            getStyle = selector;
            style = null;
        }

        string ISelectOption.GetName(IListMenuManager manager, IListMenuArg arg)
        {
            if (getName != null)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                    LuiAssert.Type<TArg>(arg, out var tArg)) return null;

                return getName(tMgr, tArg);
            }
            else return name;
        }

        string ISelectOption.GetStyle(IListMenuManager manager, IListMenuArg arg)
        {
            if (getStyle != null)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                    LuiAssert.Type<TArg>(arg, out var tArg)) return null;

                return getStyle(tMgr, tArg);
            }
            else return style;
        }

        void ISelectOption.Click(IListMenuManager manager, IListMenuArg arg)
        {
            if (LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

            Click(tMgr, tArg);
        }
    }
}
