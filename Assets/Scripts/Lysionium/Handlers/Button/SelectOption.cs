namespace Lysionium
{
    /// <inheritdoc/>
    public class SelectOption : SelectOption<IListuiManager>
    {
        public SelectOption()
        {
        }

        public SelectOption(string name, System.Action<IListuiManager> onClick, string style = null)
            : base(name, onClick, style)
        {
        }

        public static SelectOption<TMgr> Create<TMgr>(
            string name, System.Action<TMgr> onClick, string style = "")
            where TMgr : IListuiManager
        {
            var instance = new SelectOption<TMgr>();
            instance.SetName(name);
            instance.SetStyle(style);
            instance.Click = onClick;
            return instance;
        }

        public static SelectOption<TMgr> Create<TMgr>(
            System.Func<TMgr, string> getName, System.Action<TMgr> onClick, string style = "")
            where TMgr : IListuiManager
        {
            var instance = new SelectOption<TMgr>();
            instance.SetName(getName);
            instance.SetStyle(style);
            instance.Click = onClick;
            return instance;
        }

        public static SelectOption<TMgr> Create<TMgr>(
            string name, System.Action<TMgr> onClick, System.Func<TMgr, string> style)
            where TMgr : IListuiManager
        {
            var instance = new SelectOption<TMgr>();
            instance.SetName(name);
            instance.SetStyle(style);
            instance.Click = onClick;
            return instance;
        }

        public static SelectOption<TMgr> Create<TMgr>(
            System.Func<TMgr, string> getName, System.Action<TMgr> onClick, System.Func<TMgr, string> style)
            where TMgr : IListuiManager
        {
            var instance = new SelectOption<TMgr>();
            instance.SetName(getName);
            instance.SetStyle(style);
            instance.Click = onClick;
            return instance;
        }

        public static SelectOption<TMgr, TArg> Create<TMgr, TArg>(
            string name, System.Action<TMgr, TArg> onClick, string style = "")
            where TMgr : IListuiManager
        {
            var instance = new SelectOption<TMgr, TArg>();
            instance.SetName(name);
            instance.SetStyle(style);
            instance.Click = onClick;
            return instance;
        }

        public static SelectOption<TMgr, TArg> Create<TMgr, TArg>(
            System.Func<TMgr, TArg, string> getName, System.Action<TMgr, TArg> onClick, string style = "")
            where TMgr : IListuiManager
        {
            var instance = new SelectOption<TMgr, TArg>();
            instance.SetName(getName);
            instance.SetStyle(style);
            instance.Click = onClick;
            return instance;
        }

        public static SelectOption<TMgr, TArg> Create<TMgr, TArg>(
            string name, System.Action<TMgr, TArg> onClick, System.Func<TMgr, TArg, string> style)
            where TMgr : IListuiManager
        {
            var instance = new SelectOption<TMgr, TArg>();
            instance.SetName(name);
            instance.SetStyle(style);
            instance.Click = onClick;
            return instance;
        }

        public static SelectOption<TMgr, TArg> Create<TMgr, TArg>(
            System.Func<TMgr, TArg, string> getName,
            System.Action<TMgr, TArg> onClick,
            System.Func<TMgr, TArg, string> style)
            where TMgr : IListuiManager
        {
            var instance = new SelectOption<TMgr, TArg>();
            instance.SetName(getName);
            instance.SetStyle(style);
            instance.Click = onClick;
            return instance;
        }
    }

    public class SelectOption<TMgr> : ISelectOption<TMgr>
    {
        private string name;
        private System.Func<TMgr, string> getName;

        private string style;
        private System.Func<TMgr, string> getStyle;

        public System.Action<TMgr> Click { get; set; }

        public SelectOption()
        {
        }

        public SelectOption(string name, System.Action<TMgr> onClick, string style = null)
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

        public void SetName(System.Func<TMgr, string> selector)
        {
            getName = selector ?? throw new System.ArgumentNullException(nameof(selector));
            name = null;
        }

        public void SetStyle(string style)
        {
            this.style = style;
            getStyle = null;
        }

        public void SetStyle(System.Func<TMgr, string> selector)
        {
            getStyle = selector;
            style = null;
        }

        string ISelectOption<TMgr>.GetName(TMgr manager) => getName?.Invoke(manager) ?? name;
        string ISelectOption<TMgr>.GetStyle(TMgr manager) => getStyle?.Invoke(manager) ?? style;
        void ISelectOption<TMgr>.Click(TMgr manager, string clickName) => Click?.Invoke(manager);
    }

    public class SelectOption<TMgr, TArg> : ISelectOption<TMgr, TArg>
    {
        private string name;
        private System.Func<TMgr, TArg, string> getName;

        private string style;
        private System.Func<TMgr, TArg, string> getStyle;

        public System.Action<TMgr, TArg> Click { get; set; }

        public SelectOption()
        {
        }

        public SelectOption(string name, System.Action<TMgr, TArg> onClick, string style = null)
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
        void ISelectOption<TMgr, TArg>.Click(TMgr manager, string clickName, TArg arg)
            => Click?.Invoke(manager, arg);
    }
}
