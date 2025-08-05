using R3;

// 命名メモ: 名前空間は System.Reactive.Linq 風の Merge.Linq ではなく MergeExtensions
// 前者は Rx 成分が不明瞭で System.Linq のような印象も受ける
namespace Lysionium.MergeExtensions.R3
{
    public static class ViewItemHandlerBuilderMergeExtension
    {
        public static TOut Merge<TItem, TMgr, TArg, TOut>(
            this IViewItemHandlerBuilder<TItem, TMgr, TArg, TOut> builder, out Subject<MergedViewItemHandleArg<TItem, TMgr, TArg, TOut, TItem>> merged)
        {
            var _merged = merged = new Subject<MergedViewItemHandleArg<TItem, TMgr, TArg, TOut, TItem>>();

            // 生成した Subject は ViewData.Dispose で破棄
            builder.Init(() => _merged);

            // 各ハンドラを購読
            SubscribeViewItemHandler(builder, merged);
            if (builder is IButtonViewItemHandlerBuilder<TItem, TMgr, TArg, TOut> buttonBuilder)
            {
                ButtonViewItemHandlerBuilderMergeExtension.SubscribeButtonViewItemHandler(buttonBuilder, merged);
            }

            return (TOut)builder;
        }

        public static MergedViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder, TValue> ToBuilder<TItem, TMgr, TArg, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> source)
        {
            return new MergedViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder, TValue>(source);
        }

        internal static Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> Fallback<TItem, TMgr, TArg, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> source)
        {
            return source.Where(x => !((MergedViewItemHandleContext<object>)x.Context).HasResult);
        }

        public static SwitchCaseViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder, TValue> Case<TItem, TMgr, TArg, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> source, System.Func<TValue, TMgr, TArg, bool> predicate,
            System.Action<Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>>> then)
        {
            return new SwitchCaseViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder, TValue>(source).Case(predicate, then);
        }

        public static SwitchCaseViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder, TValue> Case<TItem, TMgr, TArg, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> source, System.Func<TValue, bool> predicate,
            System.Action<Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>>> then)
        {
            return new SwitchCaseViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder, TValue>(source).Case(predicate, then);
        }

        internal static Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> SwitchCase<TItem, TMgr, TArg, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> source,
            System.Action<SwitchCaseViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder, TValue>> action)
        {
            var switchCaseBuilder = new SwitchCaseViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder, TValue>(source);
            action(switchCaseBuilder);
            return source;
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> Where<TItem, TMgr, TArg, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> source, System.Func<TValue, TMgr, TArg, bool> predicate)
        {
            return source.Where(x => predicate(x.Value, x.Manager, x.Arg));
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> Where<TItem, TMgr, TArg, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> source, System.Func<TValue, bool> predicate)
        {
            return source.Where(x => predicate(x.Value));
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TOutValue>> Select<TItem, TMgr, TArg, TBuilder, TInValue, TOutValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TInValue>> source, System.Func<TInValue, TMgr, TArg, TOutValue> selector)
        {
            return source.Select(x =>
            {
                var value = selector(x.Value, x.Manager, x.Arg);
                return x.SetValue(value);
            });
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TOutValue>> Select<TItem, TMgr, TArg, TBuilder, TInValue, TOutValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TInValue>> source, System.Func<TInValue, TOutValue> selector)
        {
            return source.Select(x =>
            {
                var value = selector(x.Value);
                return x.SetValue(value);
            });
        }



        // このメソッドの戻り値を Observable にして Merge する方法もあるが少々煩雑
        public static TOut SubscribeViewItemHandler<TItem, TMgr, TArg, TOut>(
            this IViewItemHandlerBuilder<TItem, TMgr, TArg, TOut> builder, Subject<MergedViewItemHandleArg<TItem, TMgr, TArg, TOut, TItem>> subject)
        {
            if (builder == null) throw new System.ArgumentNullException(nameof(builder));
            if (subject == null) throw new System.ArgumentNullException(nameof(subject));

            var nameFromContext = new NameFromContext();
            builder.NameFrom((item, manager, arg) =>
            {
                lock (nameFromContext)
                {
                    using var _ = nameFromContext.OpenSelf();
                    subject.OnNext(new MergedViewItemHandleArg<TItem, TMgr, TArg, TOut, TItem>(item, manager, arg, nameFromContext));
                    if (nameFromContext.TryGetResult(out var result)) return result;
                    else return item?.ToString() ?? "null";
                }
            });

            var styleFromContext = new StyleFromContext();
            builder.StyleFrom((item, manager, arg) =>
            {
                lock (styleFromContext)
                {
                    using var _ = styleFromContext.OpenSelf();
                    subject.OnNext(new MergedViewItemHandleArg<TItem, TMgr, TArg, TOut, TItem>(item, manager, arg, styleFromContext));
                    if (styleFromContext.TryGetResult(out var result)) return result;
                    else return null;
                }
            });

            return (TOut)builder;
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> NameFrom<TItem, TMgr, TArg, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> source, System.Func<TValue, TMgr, TArg, string> selector)
            where TBuilder : IViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (selector == null) throw new System.ArgumentNullException(nameof(selector));

            return source.ToBuilder().SubscribeTo<NameFromContext>((value, manager, arg, context) =>
            {
                context.Result = selector(value, manager, arg);
            });
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> NameFrom<TItem, TMgr, TArg, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> source, System.Func<TValue, string> selector)
            where TBuilder : IViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (selector == null) throw new System.ArgumentNullException(nameof(selector));

            return source.ToBuilder().SubscribeTo<NameFromContext>((value, manager, arg, context) =>
            {
                context.Result = selector(value);
            });
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> StyleFrom<TItem, TMgr, TArg, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> source, System.Func<TValue, TMgr, TArg, string> selector)
            where TBuilder : IViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (selector == null) throw new System.ArgumentNullException(nameof(selector));

            return source.ToBuilder().SubscribeTo<StyleFromContext>((value, manager, arg, context) =>
            {
                context.Result = selector(value, manager, arg);
            });
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> StyleFrom<TItem, TMgr, TArg, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> source, System.Func<TValue, string> selector)
            where TBuilder : IViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (selector == null) throw new System.ArgumentNullException(nameof(selector));

            return source.ToBuilder().SubscribeTo<StyleFromContext>((value, manager, arg, context) =>
            {
                context.Result = selector(value);
            });
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> StyleFrom<TItem, TMgr, TArg, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TArg, TBuilder, TValue>> source, string style)
            where TBuilder : IViewItemHandlerBuilder<TItem, TMgr, TArg, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));

            return source.ToBuilder().SubscribeTo<StyleFromContext>((value, manager, arg, context) =>
            {
                context.Result = style;
            });
        }

        private class NameFromContext : MergedViewItemHandleContext<string> { }
        private class StyleFromContext : MergedViewItemHandleContext<string> { }
    }
}
