namespace Lysionium
{
    public static class ListuiScreenManagerExtensions
    {
        public static TBuilder Option<TMgr, TArg, TBuilder>(
            this ISelectOptionListBuilder<TMgr, TArg, TBuilder> builder,
            string name, IListuiScreen<TMgr, TArg> screen, string style = null)
            where TMgr : IListuiScreenManager<TMgr, TArg>
            where TArg : IListuiArg
        {
            return builder.Option(name, (manager, arg) => manager.PushScreen(screen, arg), style);
        }

        public static TBuilder Option<TMgr, TArg, TBuilder>(
            this ISelectOptionListBuilder<TMgr, TArg, TBuilder> builder,
            System.Func<TMgr, TArg, string> getName, IListuiScreen<TMgr, TArg> screen, string style = null)
            where TMgr : IListuiScreenManager<TMgr, TArg>
            where TArg : IListuiArg
        {
            return builder.Option(getName, (manager, arg) => manager.PushScreen(screen, arg), style);
        }
    }
}
