using System.Collections.Generic;

namespace Lysionium
{
    public static class SelectOptionsBuilderExtensions
    {
        public static TBuilder OptionRange<TMgr, TArg, TBuilder>(
            this ISelectOptionsBuilder<TMgr, TArg, TBuilder> builder, IEnumerable<ISelectOption<TMgr, TArg>> options)
            where TMgr : IListuiManager
            where TArg : IListuiArg
        {
            foreach (var option in options)
            {
                builder.Option(option);
            }

            // ListViewData.BaseListBuilder.HeadBuilder などビルダーの型と戻り値が一致しない可能性があるためキャストは禁止
            return builder.Option();
        }

        public static TBuilder Option<TMgr, TArg, TBuilder>(
            this ISelectOptionsBuilder<TMgr, TArg, TBuilder> builder,
            string name, ClickItemHandler<TMgr, TArg> onClick, string style = null)
            where TMgr : IListuiManager
            where TArg : IListuiArg
        {
            return builder.Option(SelectOption.Create(name, onClick, style));
        }

        public static TBuilder Option<TMgr, TArg, TBuilder>(
            this ISelectOptionsBuilder<TMgr, TArg, TBuilder> builder,
            System.Func<TMgr, TArg, string> getName, ClickItemHandler<TMgr, TArg> onClick, string style = null)
            where TMgr : IListuiManager
            where TArg : IListuiArg
        {
            return builder.Option(SelectOption.Create(getName, onClick, style));
        }
    }
}
