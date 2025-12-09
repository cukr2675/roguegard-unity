namespace Lysionium
{
    public static class BackSelectOptionListBuilderExtensions
    {
        public static TBuilder Back<TMgr, TArg, TBuilder>(
            this ISelectOptionListBuilder<TMgr, TArg, TBuilder> builder, string name = null, string style = null)
            where TMgr : IBackOptionProviderListMenuManager<TMgr, TArg>
            where TArg : IListMenuArg
        {
            if (name == null && style == null)
            {
                return builder.Option(BackSelectOption<TMgr, TArg>.Instance);
            }
            else
            {
                return builder.Option(new BackSelectOption<TMgr, TArg>(name, style));
            }
        }

        // 設計メモ: あまり自由に使ってほしくないので引数は作らない
        public static SelectOptionList<TMgr, TArg> BackIfReflectable<TMgr, TArg>(this SelectOptionList<TMgr, TArg> builder)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            if (BackSelectOption.TryCreate<TMgr, TArg>(out var backOption))
            {
                return builder.Option(backOption);
            }
            return builder;
        }
    }
}
