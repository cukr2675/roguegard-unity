namespace Lysionium
{
    public static class ListuiScreenManagerExtensions
    {
        public static TBuilder Option<TMgr, TBuilder>(
            this ISelectOptionsBuilder<TMgr, TBuilder> builder,
            string name, IListuiScreen<TMgr> screen, string style = null)
            where TMgr : IListuiScreenManager<TMgr>
        {
            return builder.Option(name, m => m.PushScreen(screen), style);
        }

        public static TBuilder Option<TMgr, TBuilder>(
            this ISelectOptionsBuilder<TMgr, TBuilder> builder,
            System.Func<TMgr, string> getName, IListuiScreen<TMgr> screen, string style = null)
            where TMgr : IListuiScreenManager<TMgr>
        {
            return builder.Option(getName, m => m.PushScreen(screen), style);
        }

        public static TBuilder Option<TMgr, TArg, TBuilder>(
            this ISelectOptionsBuilder<TMgr, TBuilder> builder,
            string name, IListuiScreen<TMgr, TArg> screen, System.Func<TArg> args, string style = null)
            where TMgr : IListuiScreenManager<TMgr>
        {
            if (args == null) throw new System.ArgumentNullException(nameof(args));

            return builder.Option(name, m => m.PushScreen(screen, args()), style);
        }

        public static TBuilder Option<TMgr, TArg, TBuilder>(
            this ISelectOptionsBuilder<TMgr, TBuilder> builder,
            System.Func<TMgr, string> getName, IListuiScreen<TMgr, TArg> screen, System.Func<TArg> args, string style = null)
            where TMgr : IListuiScreenManager<TMgr>
        {
            if (args == null) throw new System.ArgumentNullException(nameof(args));

            return builder.Option(getName, m => m.PushScreen(screen, args()), style);
        }
    }
}
