using System.Collections.Generic;

namespace Lysionium
{
    public static class SelectOptionsBuilderExtensions
    {
        public static TBuilder Option<TMgr, TArg, TBuilder>(
            this ISelectOptionsBuilder<TMgr, TBuilder> builder, ISelectOption<TMgr, TArg> option, System.Func<TArg> args)
            where TMgr : IListuiManager
        {
            if (args == null) throw new System.ArgumentNullException(nameof(args));

            return builder.Option(SelectOption.Create<TMgr>(
                m => option.GetName(m, args()),
                m => option.Click(m, args()),
                m => option.GetStyle(m, args())));
        }

        public static TBuilder OptionRange<TMgr, TBuilder>(
            this ISelectOptionsBuilder<TMgr, TBuilder> builder, IEnumerable<ISelectOption<TMgr>> options)
            where TMgr : IListuiManager
        {
            foreach (var option in options)
            {
                builder.Option(option);
            }

            // ListViewData.BaseListBuilder.HeadBuilder などビルダーの型と戻り値が一致しない可能性があるためキャストは禁止
            return builder.Option();
        }

        public static TBuilder Option<TMgr, TBuilder>(
            this ISelectOptionsBuilder<TMgr, TBuilder> builder,
            string name, ClickOptionHandler<TMgr> onClick, string style = null)
            where TMgr : IListuiManager
        {
            return builder.Option(SelectOption.Create(name, onClick, style));
        }

        public static TBuilder Option<TMgr, TBuilder>(
            this ISelectOptionsBuilder<TMgr, TBuilder> builder,
            System.Func<TMgr, string> getName, ClickOptionHandler<TMgr> onClick, string style = null)
            where TMgr : IListuiManager
        {
            return builder.Option(SelectOption.Create(getName, onClick, style));
        }

        public static TBuilder OptionRange<TMgr, TArg, TBuilder>(
            this ISelectOptionsBuilder<TMgr, TArg, TBuilder> builder, IEnumerable<ISelectOption<TMgr, TArg>> options)
            where TMgr : IListuiManager
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
            string name, ClickOptionHandler<TMgr, TArg> onClick, string style = null)
            where TMgr : IListuiManager
        {
            return builder.Option(SelectOption.Create(name, onClick, style));
        }

        public static TBuilder Option<TMgr, TArg, TBuilder>(
            this ISelectOptionsBuilder<TMgr, TArg, TBuilder> builder,
            System.Func<TMgr, TArg, string> getName, ClickOptionHandler<TMgr, TArg> onClick, string style = null)
            where TMgr : IListuiManager
        {
            return builder.Option(SelectOption.Create(getName, onClick, style));
        }
    }
}
