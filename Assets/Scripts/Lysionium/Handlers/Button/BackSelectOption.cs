namespace Lysionium
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
            return new BackSelectOption
            {
                name = name,
                style = style
            };
        }

        string ISelectOption.GetName(IListMenuManager manager, IListMenuArg arg) => name ?? manager.BackOption.GetName(manager, arg);
        string ISelectOption.GetStyle(IListMenuManager manager, IListMenuArg arg) => style ?? manager.BackOption.GetStyle(manager, arg);
        void ISelectOption.Click(IListMenuManager manager, IListMenuArg arg) => manager.BackOption.Click(manager, arg);
    }
}
