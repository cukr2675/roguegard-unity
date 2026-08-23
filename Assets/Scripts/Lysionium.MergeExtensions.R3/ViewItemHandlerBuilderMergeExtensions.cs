using R3;

// 命名メモ: 名前空間は System.Reactive.Linq 風の Merge.Linq ではなく MergeExtensions
// 前者は Rx 成分が不明瞭で System.Linq のような印象も受ける
namespace Lysionium.MergeExtensions.R3
{
    public static class ViewItemHandlerBuilderMergeExtensions
    {
        public static TOut Merge<TItem, TMgr, TOut>(
            this IViewItemHandlerBuilder<TItem, TMgr, TOut> builder, out Subject<MergedViewItemHandleArg<TItem, TMgr, TOut, TItem>> merged)
        {
            var _merged = merged = new Subject<MergedViewItemHandleArg<TItem, TMgr, TOut, TItem>>();

            // 生成した Subject は ViewData.Dispose で破棄
            builder.Init(() => _merged);

            // 各ハンドラを購読
            SubscribeViewItemHandler(builder, merged);
            if (builder is IEventGestureViewItemHandlerBuilder<TItem, TMgr, TOut> buttonBuilder)
            {
                buttonBuilder.SubscribeEventGestureViewItemHandler(merged);
            }
            if (builder is IViewItemFilterBuilder<TItem, TMgr, TOut> filterBuilder)
            {
                filterBuilder.SubscribeViewItemFilter(merged);
            }

            return (TOut)builder;
        }

        public static MergedViewItemHandlerBuilder<TItem, TMgr, TBuilder, TValue> ToBuilder<TItem, TMgr, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> source)
        {
            return new MergedViewItemHandlerBuilder<TItem, TMgr, TBuilder, TValue>(source);
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> VarOnce<TItem, TMgr, TBuilder, TValue, TVar>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> source, out TVar variable, TVar defaultValue = default)
        {
            variable = defaultValue;
            return source;
        }

        internal static Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> Fallback<TItem, TMgr, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> source)
        {
            return source.Where(x => !((MergedViewItemHandleContext<object>)x.Context).HasResult);
        }

        public static SwitchCaseViewItemHandlerBuilder<TItem, TMgr, TBuilder, TValue> Case<TItem, TMgr, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> source, System.Func<TValue, TMgr, bool> predicate,
            System.Action<Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>>> then)
        {
            return new SwitchCaseViewItemHandlerBuilder<TItem, TMgr, TBuilder, TValue>(source).Case(predicate, then);
        }

        public static SwitchCaseViewItemHandlerBuilder<TItem, TMgr, TBuilder, TValue> Case<TItem, TMgr, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> source, System.Func<TValue, bool> predicate,
            System.Action<Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>>> then)
        {
            return new SwitchCaseViewItemHandlerBuilder<TItem, TMgr, TBuilder, TValue>(source).Case(predicate, then);
        }

        internal static Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> SwitchCase<TItem, TMgr, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> source,
            System.Action<SwitchCaseViewItemHandlerBuilder<TItem, TMgr, TBuilder, TValue>> action)
        {
            var switchCaseBuilder = new SwitchCaseViewItemHandlerBuilder<TItem, TMgr, TBuilder, TValue>(source);
            action(switchCaseBuilder);
            return source;
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> Where<TItem, TMgr, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> source, System.Func<TValue, TMgr, bool> predicate)
        {
            return source.Where(x => predicate(x.Value, x.Manager));
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> Where<TItem, TMgr, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> source, System.Func<TValue, bool> predicate)
        {
            return source.Where(x => predicate(x.Value));
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TOutValue>> Select<TItem, TMgr, TBuilder, TInValue, TOutValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TInValue>> source, System.Func<TInValue, TMgr, TOutValue> selector)
        {
            return source.Select(x =>
            {
                var value = selector(x.Value, x.Manager);
                return x.SetValue(value);
            });
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TOutValue>> Select<TItem, TMgr, TBuilder, TInValue, TOutValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TInValue>> source, System.Func<TInValue, TOutValue> selector)
        {
            return source.Select(x =>
            {
                var value = selector(x.Value);
                return x.SetValue(value);
            });
        }



        // このメソッドの戻り値を Observable にして Merge する方法もあるが少々煩雑
        public static TOut SubscribeViewItemHandler<TItem, TMgr, TOut>(
            this IViewItemHandlerBuilder<TItem, TMgr, TOut> builder, Subject<MergedViewItemHandleArg<TItem, TMgr, TOut, TItem>> subject)
        {
            if (builder == null) throw new System.ArgumentNullException(nameof(builder));
            if (subject == null) throw new System.ArgumentNullException(nameof(subject));

            var nameFromContext = new NameFromContext();
            builder.NameFrom((item, manager) =>
            {
                lock (nameFromContext)
                {
                    using var _ = nameFromContext.OpenSelf();
                    subject.OnNext(new MergedViewItemHandleArg<TItem, TMgr, TOut, TItem>(item, manager, nameFromContext));
                    if (nameFromContext.TryGetResult(out var result)) return result;
                    else return item?.ToString() ?? "null";
                }
            });

            var styleFromContext = new StyleFromContext();
            builder.StyleFrom((item, manager) =>
            {
                lock (styleFromContext)
                {
                    using var _ = styleFromContext.OpenSelf();
                    subject.OnNext(new MergedViewItemHandleArg<TItem, TMgr, TOut, TItem>(item, manager, styleFromContext));
                    if (styleFromContext.TryGetResult(out var result)) return result;
                    else return string.Empty;
                }
            });

            return (TOut)builder;
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> NameFrom<TItem, TMgr, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> source, System.Func<TValue, TMgr, string> selector)
            where TBuilder : IViewItemHandlerBuilder<TItem, TMgr, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (selector == null) throw new System.ArgumentNullException(nameof(selector));

            return source.ToBuilder().SubscribeTo<NameFromContext>((value, manager, context) =>
            {
                context.Result = selector(value, manager);
            });
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> NameFrom<TItem, TMgr, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> source, System.Func<TValue, string> selector)
            where TBuilder : IViewItemHandlerBuilder<TItem, TMgr, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (selector == null) throw new System.ArgumentNullException(nameof(selector));

            return source.ToBuilder().SubscribeTo<NameFromContext>((value, manager, context) =>
            {
                context.Result = selector(value);
            });
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> StyleFrom<TItem, TMgr, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> source, System.Func<TValue, TMgr, string> selector)
            where TBuilder : IViewItemHandlerBuilder<TItem, TMgr, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (selector == null) throw new System.ArgumentNullException(nameof(selector));

            return source.ToBuilder().SubscribeTo<StyleFromContext>((value, manager, context) =>
            {
                context.Result = selector(value, manager);
            });
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> StyleFrom<TItem, TMgr, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> source, System.Func<TValue, string> selector)
            where TBuilder : IViewItemHandlerBuilder<TItem, TMgr, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (selector == null) throw new System.ArgumentNullException(nameof(selector));

            return source.ToBuilder().SubscribeTo<StyleFromContext>((value, manager, context) =>
            {
                context.Result = selector(value);
            });
        }

        public static Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> Style<TItem, TMgr, TBuilder, TValue>(
            this Observable<MergedViewItemHandleArg<TItem, TMgr, TBuilder, TValue>> source, string style)
            where TBuilder : IViewItemHandlerBuilder<TItem, TMgr, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));

            return source.ToBuilder().SubscribeTo<StyleFromContext>((value, manager, context) =>
            {
                context.Result = style;
            });
        }

        private class NameFromContext : MergedViewItemHandleContext<string> { }
        private class StyleFromContext : MergedViewItemHandleContext<string> { }
    }
}
