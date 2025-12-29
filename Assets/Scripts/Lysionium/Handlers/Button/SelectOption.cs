namespace Lysionium
{
    /// <inheritdoc/>
    public class SelectOption : SelectOption<IListuiManager, IListuiArg>
    {
        public SelectOption()
        {
        }

        public SelectOption(string name, ClickItemHandler<IListuiManager, IListuiArg> onClick, string style = null)
            : base(name, onClick, style)
        {
        }

        public static SelectOption<TMgr, TArg> Create<TMgr, TArg>(
            string name, ClickItemHandler<TMgr, TArg> onClick, string style = "")
            where TMgr : IListuiManager
            where TArg : IListuiArg
        {
            var instance = new SelectOption<TMgr, TArg>();
            instance.SetName(name);
            instance.SetStyle(style);
            instance.Click = onClick;
            return instance;
        }

        public static SelectOption<TMgr, TArg> Create<TMgr, TArg>(
            System.Func<TMgr, TArg, string> getName, ClickItemHandler<TMgr, TArg> onClick, string style = "")
            where TMgr : IListuiManager
            where TArg : IListuiArg
        {
            var instance = new SelectOption<TMgr, TArg>();
            instance.SetName(getName);
            instance.SetStyle(style);
            instance.Click = onClick;
            return instance;
        }

        public static SelectOption<TMgr, TArg> Create<TMgr, TArg>(
            string name, ClickItemHandler<TMgr, TArg> onClick, System.Func<TMgr, TArg, string> style)
            where TMgr : IListuiManager
            where TArg : IListuiArg
        {
            var instance = new SelectOption<TMgr, TArg>();
            instance.SetName(name);
            instance.SetStyle(style);
            instance.Click = onClick;
            return instance;
        }

        public static SelectOption<TMgr, TArg> Create<TMgr, TArg>(
            System.Func<TMgr, TArg, string> getName, ClickItemHandler<TMgr, TArg> onClick, System.Func<TMgr, TArg, string> style)
            where TMgr : IListuiManager
            where TArg : IListuiArg
        {
            var instance = new SelectOption<TMgr, TArg>();
            instance.SetName(getName);
            instance.SetStyle(style);
            instance.Click = onClick;
            return instance;
        }
    }

    /// <inheritdoc/>
    public class SelectOption<TMgr> : SelectOption<TMgr, IListuiArg>
    {
        public SelectOption()
        {
        }

        public SelectOption(string name, ClickItemHandler<TMgr, IListuiArg> onClick, string style = null)
            : base(name, onClick, style)
        {
        }
    }

    public class SelectOption<TMgr, TArg> : ISelectOption<TMgr, TArg>
    {
        private string name;
        private System.Func<TMgr, TArg, string> getName;

        private string style;
        private System.Func<TMgr, TArg, string> getStyle;

        public ClickItemHandler<TMgr, TArg> Click { get; set; }

        public SelectOption()
        {
        }

        public SelectOption(string name, ClickItemHandler<TMgr, TArg> onClick, string style = null)
        {
            this.name = name;
            Click = onClick;
            this.style = style;
        }

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

        string ISelectOption<TMgr, TArg>.GetName(TMgr manager, TArg arg) => getName?.Invoke(manager, arg) ?? name;
        string ISelectOption<TMgr, TArg>.GetStyle(TMgr manager, TArg arg) => getStyle?.Invoke(manager, arg) ?? style;
        void ISelectOption<TMgr, TArg>.Click(TMgr manager, TArg arg) => Click(manager, arg);
    }
}
