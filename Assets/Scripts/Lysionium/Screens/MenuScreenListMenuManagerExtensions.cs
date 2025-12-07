namespace Lysionium
{
    public static class MenuScreenListMenuManagerExtensions
    {
        public static TBuilder Option<TMgr, TArg, TBuilder>(
            this ISelectOptionListBuilder<TMgr, TArg, TBuilder> builder,
            string name, IMenuScreen<TMgr, TArg> menuScreen, string style = null)
            where TMgr : IMenuScreenListMenuManager<TMgr, TArg>
            where TArg : IListMenuArg
        {
            return builder.Option(name, (manager, arg) => manager.PushMenuScreen(menuScreen, arg), style);
        }

        public static TBuilder Option<TMgr, TArg, TBuilder>(
            this ISelectOptionListBuilder<TMgr, TArg, TBuilder> builder,
            System.Func<TMgr, TArg, string> getName, IMenuScreen<TMgr, TArg> menuScreen, string style = null)
            where TMgr : IMenuScreenListMenuManager<TMgr, TArg>
            where TArg : IListMenuArg
        {
            return builder.Option(getName, (manager, arg) => manager.PushMenuScreen(menuScreen, arg), style);
        }
    }
}
