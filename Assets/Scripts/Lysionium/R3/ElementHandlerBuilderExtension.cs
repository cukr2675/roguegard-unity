using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using R3;

namespace Lysionium.R3
{
    public static class ElementHandlerBuilderExtension
    {
        public static TOut R3<TElm, TMgr, TArg, TOut>(
            this IElementHandlerBuilder<TElm, TMgr, TArg, TOut> builder, out Subject<R3RuleArg<TElm, TMgr, TArg, TOut, TElm>> subject)
        {
            subject = new Subject<R3RuleArg<TElm, TMgr, TArg, TOut, TElm>>();
            SubscribeElementHandler(builder, subject);

            if (builder is IButtonElementHandlerBuilder<TElm, TMgr, TArg, TOut> buttonsBuilder)
            {
                ButtonElementHandlerBuilderExtension.SubscribeButtonElementHandler(buttonsBuilder, subject);
            }

            return (TOut)builder;
        }

        public static ObservedHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> Handle<TElm, TMgr, TArg, TBuilder, TValue>(
            this Observable<R3RuleArg<TElm, TMgr, TArg, TBuilder, TValue>> source)
        {
            return ObservedHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue>.Handle(source, true);
        }

        public static ObservedHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> WithoutHandle<TElm, TMgr, TArg, TBuilder, TValue>(
            this Observable<R3RuleArg<TElm, TMgr, TArg, TBuilder, TValue>> source)
        {
            return ObservedHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue>.Handle(source, false);
        }

        public static Observable<R3RuleArg<TElm, TMgr, TArg, TBuilder, TValue>> NotHandled<TElm, TMgr, TArg, TBuilder, TValue>(
            this Observable<R3RuleArg<TElm, TMgr, TArg, TBuilder, TValue>> source)
        {
            return source.Where(x => !x.Ctx.IsHandled);
        }

        internal static Observable<R3RuleArg<TElm, TMgr, TArg, TBuilder, TValue>> Fallback<TElm, TMgr, TArg, TBuilder, TValue>(
            this Observable<R3RuleArg<TElm, TMgr, TArg, TBuilder, TValue>> source)
        {
            return source.Where(x => ((R3RuleContext<object>)x.Ctx).ReturnValue == null);
        }

        public static Observable<R3RuleArg<TElm, TMgr, TArg, TBuilder, TValue>> Where<TElm, TMgr, TArg, TBuilder, TValue>(
            this Observable<R3RuleArg<TElm, TMgr, TArg, TBuilder, TValue>> source, System.Func<TValue, TMgr, TArg, bool> predicate)
        {
            return source.Where(x => predicate(x.Value, x.Manager, x.Arg));
        }

        public static Observable<R3RuleArg<TElm, TMgr, TArg, TBuilder, TValue>> WhereElm<TElm, TMgr, TArg, TBuilder, TValue>(
            this Observable<R3RuleArg<TElm, TMgr, TArg, TBuilder, TValue>> source, System.Func<TValue, bool> predicate)
        {
            return source.Where(x => predicate(x.Value));
        }

        public static Observable<R3RuleArg<TElm, TMgr, TArg, TBuilder, TOutValue>> Select<TElm, TMgr, TArg, TBuilder, TInValue, TOutValue>(
            this Observable<R3RuleArg<TElm, TMgr, TArg, TBuilder, TInValue>> source, System.Func<TInValue, TMgr, TArg, TOutValue> selector)
        {
            return source.Select(x =>
            {
                var value = selector(x.Value, x.Manager, x.Arg);
                return x.SetValue(value);
            });
        }

        public static Observable<R3RuleArg<TElm, TMgr, TArg, TBuilder, TOutValue>> SelectElm<TElm, TMgr, TArg, TBuilder, TInValue, TOutValue>(
            this Observable<R3RuleArg<TElm, TMgr, TArg, TBuilder, TInValue>> source, System.Func<TInValue, TOutValue> selector)
        {
            return source.Select(x =>
            {
                var value = selector(x.Value);
                return x.SetValue(value);
            });
        }



        public static TOut SubscribeElementHandler<TElm, TMgr, TArg, TOut>(
            this IElementHandlerBuilder<TElm, TMgr, TArg, TOut> builder, Subject<R3RuleArg<TElm, TMgr, TArg, TOut, TElm>> subject)
        {
            if (builder == null) throw new System.ArgumentNullException(nameof(builder));
            if (subject == null) throw new System.ArgumentNullException(nameof(subject));

            var nameFromCtx = new NameFromContext();
            builder.NameFrom((element, manager, arg) =>
            {
                using var _ = nameFromCtx.OpenSelf();
                subject.OnNext(new R3RuleArg<TElm, TMgr, TArg, TOut, TElm>(element, manager, arg, nameFromCtx));
                return nameFromCtx.ReturnValue;
            });

            var styleFromCtx = new StyleFromContext();
            builder.StyleFrom((element, manager, arg) =>
            {
                using var _ = styleFromCtx.OpenSelf();
                subject.OnNext(new R3RuleArg<TElm, TMgr, TArg, TOut, TElm>(element, manager, arg, styleFromCtx));
                return styleFromCtx.ReturnValue;
            });

            return (TOut)builder;
        }

        /// <summary>
        /// 暗黙の Handle() 呼び出し
        /// </summary>
        public static ObservedHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> NameFrom<TElm, TMgr, TArg, TBuilder, TValue>(
            this Observable<R3RuleArg<TElm, TMgr, TArg, TBuilder, TValue>> source, GetElementName<TValue, TMgr, TArg> nameFrom)
            where TBuilder : IElementHandlerBuilder<TElm, TMgr, TArg, TBuilder>
            => source.Handle().NameFrom(nameFrom);

        public static ObservedHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> NameFrom<TElm, TMgr, TArg, TBuilder, TValue>(
            this ObservedHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> source, GetElementName<TValue, TMgr, TArg> nameFrom)
            where TBuilder : IElementHandlerBuilder<TElm, TMgr, TArg, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (nameFrom == null) throw new System.ArgumentNullException(nameof(nameFrom));

            source.SubscribeOf<NameFromContext>((value, manager, arg, ctx) =>
            {
                ctx.ReturnValue = nameFrom(value, manager, arg);
            });
            return source;
        }

        /// <summary>
        /// 暗黙の Handle() 呼び出し
        /// </summary>
        public static ObservedHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> StyleFrom<TElm, TMgr, TArg, TBuilder, TValue>(
            this Observable<R3RuleArg<TElm, TMgr, TArg, TBuilder, TValue>> source, GetElementName<TValue, TMgr, TArg> styleFrom)
            where TBuilder : IElementHandlerBuilder<TElm, TMgr, TArg, TBuilder>
            => source.Handle().StyleFrom(styleFrom);

        public static ObservedHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> StyleFrom<TElm, TMgr, TArg, TBuilder, TValue>(
            this ObservedHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> source, GetElementName<TValue, TMgr, TArg> styleFrom)
            where TBuilder : IElementHandlerBuilder<TElm, TMgr, TArg, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (styleFrom == null) throw new System.ArgumentNullException(nameof(styleFrom));

            source.SubscribeOf<StyleFromContext>((value, manager, arg, ctx) =>
            {
                ctx.ReturnValue = styleFrom(value, manager, arg);
            });
            return source;
        }

        /// <summary>
        /// 暗黙の Handle() 呼び出し
        /// </summary>
        public static ObservedHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> StyleFrom<TElm, TMgr, TArg, TBuilder, TValue>(
            this Observable<R3RuleArg<TElm, TMgr, TArg, TBuilder, TValue>> source, string style)
            where TBuilder : IElementHandlerBuilder<TElm, TMgr, TArg, TBuilder>
            => source.Handle().StyleFrom(style);

        public static ObservedHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> StyleFrom<TElm, TMgr, TArg, TBuilder, TValue>(
            this ObservedHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> source, string style)
            where TBuilder : IElementHandlerBuilder<TElm, TMgr, TArg, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));

            source.SubscribeOf<StyleFromContext>((value, manager, arg, ctx) =>
            {
                ctx.ReturnValue = style;
            });
            return source;
        }

        private class NameFromContext : R3RuleContext<string> { }
        private class StyleFromContext : R3RuleContext<string> { }
    }
}
