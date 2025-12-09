using System.Collections.Generic;

namespace Lysionium
{
    public static class SelectOptionListBuilderExtensions
    {
        public static TBuilder OptionRange<TMgr, TArg, TBuilder>(
            this ISelectOptionListBuilder<TMgr, TArg, TBuilder> builder, IEnumerable<ISelectOption<TMgr, TArg>> options)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            foreach (var option in options)
            {
                builder.Option(option);
            }
            return (TBuilder)builder;
        }

        public static TBuilder Option<TMgr, TArg, TBuilder>(
            this ISelectOptionListBuilder<TMgr, TArg, TBuilder> builder,
            string name, ClickItemHandler<TMgr, TArg> onClick, string style = null)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            return builder.Option(SelectOption.Create(name, onClick, style));
        }

        public static TBuilder Option<TMgr, TArg, TBuilder>(
            this ISelectOptionListBuilder<TMgr, TArg, TBuilder> builder,
            System.Func<TMgr, TArg, string> getName, ClickItemHandler<TMgr, TArg> onClick, string style = null)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            return builder.Option(SelectOption.Create(getName, onClick, style));
        }
    }
}
