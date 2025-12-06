namespace Lysionium
{
    public static class SelectOptionListBuilderExtensions
    {
        public static TBuilder Option<TMgr, TArg, TBuilder>(
            this ISelectOptionListBuilder<TMgr, TArg, TBuilder> builder, string name, ClickItemHandler<TMgr, TArg> onClick, string style = null)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            return builder.Option(SelectOption.Create(name, onClick, style));
        }

        public static TBuilder Back<TMgr, TArg, TBuilder>(
            this ISelectOptionListBuilder<TMgr, TArg, TBuilder> builder, string name = null, string style = null)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            if (name == null && style == null)
            {
                return builder.Option(BackSelectOption.Instance);
            }
            else
            {
                return builder.Option(BackSelectOption.Create<TMgr, TArg>(name, style));
            }
        }
    }
}
