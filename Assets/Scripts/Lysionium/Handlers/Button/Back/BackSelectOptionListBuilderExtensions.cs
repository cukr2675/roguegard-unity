namespace Lysionium
{
    public static class BackSelectOptionListBuilderExtensions
    {
        public static TBuilder Back<TMgr, TBuilder>(
            this ISelectOptionsBuilder<TMgr, TBuilder> builder, string name = null, string style = null)
            where TMgr : IBackOptionProviderListuiManager<TMgr>
        {
            if (name == null && style == null)
            {
                return builder.Option(BackSelectOption<TMgr>.Instance);
            }
            else
            {
                return builder.Option(new BackSelectOption<TMgr>(name, style));
            }
        }

        // 設計メモ: あまり自由に使ってほしくないので引数は作らない
        public static SelectOptionList<TMgr> BackIfReflectable<TMgr>(this SelectOptionList<TMgr> builder)
            where TMgr : IListuiManager
        {
            if (BackSelectOption.TryCreate<TMgr>(out var backOption))
            {
                return builder.Option(backOption);
            }
            return builder;
        }
    }
}
