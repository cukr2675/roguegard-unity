using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium.HandlerRules
{
    public static class ElementHandlerBuilderExtension
    {
        public static TOut Rule<TElm, TMgr, TArg, TOut>(
            this IViewItemHandlerBuilder<TElm, TMgr, TArg, TOut> builder, out HandlerSubject<TElm, TMgr, TArg, TOut> subject)
        {
            subject = new HandlerSubject<TElm, TMgr, TArg, TOut>();
            SubscribeElementHandler(builder, subject);

            if (builder is IButtonViewItemHandlerBuilder<TElm, TMgr, TArg, TOut> buttonsBuilder)
            {
                ButtonElementHandlerBuilderExtension.SubscribeButtonElementHandler(buttonsBuilder, subject);
            }

            return (TOut)builder;
        }

        public static RuledHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> Handle<TElm, TMgr, TArg, TBuilder, TValue>(
            this IRulable<TElm, TMgr, TArg, TBuilder, TValue> source)
        {
            return RuledHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue>.Handle(source, true);
        }

        public static RuledHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> WithoutHandle<TElm, TMgr, TArg, TBuilder, TValue>(
            this IRulable<TElm, TMgr, TArg, TBuilder, TValue> source)
        {
            return RuledHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue>.Handle(source, false);
        }

        public static IRulable<TElm, TMgr, TArg, TBuilder, TValue> NotHandled<TElm, TMgr, TArg, TBuilder, TValue>(
            this IRulable<TElm, TMgr, TArg, TBuilder, TValue> source)
        {
            return new Where<TElm, TMgr, TArg, TBuilder, TValue>(source, (_, _, _, ctx) => !ctx.IsHandled);
        }

        internal static IRulable<TElm, TMgr, TArg, TBuilder, TValue> Fallback<TElm, TMgr, TArg, TBuilder, TValue>(
            this IRulable<TElm, TMgr, TArg, TBuilder, TValue> source)
        {
            return new Where<TElm, TMgr, TArg, TBuilder, TValue>(source, (_, _, _, ctx) => ((RuleContext<object>)ctx).ReturnValue == null);
        }

        public static IRulable<TElm, TMgr, TArg, TBuilder, TValue> Where<TElm, TMgr, TArg, TBuilder, TValue>(
            this IRulable<TElm, TMgr, TArg, TBuilder, TValue> source, System.Func<TValue, TMgr, TArg, bool> predicate)
        {
            return new Where<TElm, TMgr, TArg, TBuilder, TValue>(source, (value, manager, arg, _) => predicate(value, manager, arg));
        }

        public static IRulable<TElm, TMgr, TArg, TBuilder, TValue> WhereElm<TElm, TMgr, TArg, TBuilder, TValue>(
            this IRulable<TElm, TMgr, TArg, TBuilder, TValue> source, System.Func<TValue, bool> predicate)
        {
            return new Where<TElm, TMgr, TArg, TBuilder, TValue>(source, (value, _, _, _) => predicate(value));
        }

        public static IRulable<TElm, TMgr, TArg, TBuilder, TOutValue> Select<TElm, TMgr, TArg, TBuilder, TInValue, TOutValue>(
            this IRulable<TElm, TMgr, TArg, TBuilder, TInValue> source, System.Func<TInValue, TMgr, TArg, TOutValue> selector)
        {
            return new Select<TElm, TMgr, TArg, TBuilder, TInValue, TOutValue>(source, (value, manager, arg, _) => selector(value, manager, arg));
        }

        public static IRulable<TElm, TMgr, TArg, TBuilder, TOutValue> SelectElm<TElm, TMgr, TArg, TBuilder, TInValue, TOutValue>(
            this IRulable<TElm, TMgr, TArg, TBuilder, TInValue> source, System.Func<TInValue, TOutValue> selector)
        {
            return new Select<TElm, TMgr, TArg, TBuilder, TInValue, TOutValue>(source, (value, _, _, _) => selector(value));
        }



        public static TOut SubscribeElementHandler<TElm, TMgr, TArg, TOut>(
            this IViewItemHandlerBuilder<TElm, TMgr, TArg, TOut> builder, HandlerSubject<TElm, TMgr, TArg, TOut> subject)
        {
            if (builder == null) throw new System.ArgumentNullException(nameof(builder));
            if (subject == null) throw new System.ArgumentNullException(nameof(subject));

            var nameFromCtx = new NameFromContext();
            builder.NameFrom((element, manager, arg) =>
            {
                lock (nameFromCtx)
                {
                    using var _ = nameFromCtx.OpenSelf();
                    subject.OnNext(element, manager, arg, nameFromCtx);
                    return nameFromCtx.ReturnValue;
                }
            });

            var styleFromCtx = new StyleFromContext();
            builder.StyleFrom((element, manager, arg) =>
            {
                lock (styleFromCtx)
                {
                    using var _ = styleFromCtx.OpenSelf();
                    subject.OnNext(element, manager, arg, styleFromCtx);
                    return styleFromCtx.ReturnValue;
                }
            });

            return (TOut)builder;
        }

        /// <summary>
        /// 暗黙の Handle() 呼び出し
        /// </summary>
        public static RuledHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> NameFrom<TElm, TMgr, TArg, TBuilder, TValue>(
            this IRulable<TElm, TMgr, TArg, TBuilder, TValue> source, ItemNameSelector<TValue, TMgr, TArg> nameFrom)
            where TBuilder : IViewItemHandlerBuilder<TElm, TMgr, TArg, TBuilder>
            => source.Handle().NameFrom(nameFrom);

        public static RuledHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> NameFrom<TElm, TMgr, TArg, TBuilder, TValue>(
            this RuledHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> source, ItemNameSelector<TValue, TMgr, TArg> nameFrom)
            where TBuilder : IViewItemHandlerBuilder<TElm, TMgr, TArg, TBuilder>
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
        public static RuledHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> StyleFrom<TElm, TMgr, TArg, TBuilder, TValue>(
            this IRulable<TElm, TMgr, TArg, TBuilder, TValue> source, ItemNameSelector<TValue, TMgr, TArg> styleFrom)
            where TBuilder : IViewItemHandlerBuilder<TElm, TMgr, TArg, TBuilder>
            => source.Handle().StyleFrom(styleFrom);

        public static RuledHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> StyleFrom<TElm, TMgr, TArg, TBuilder, TValue>(
            this RuledHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> source, ItemNameSelector<TValue, TMgr, TArg> styleFrom)
            where TBuilder : IViewItemHandlerBuilder<TElm, TMgr, TArg, TBuilder>
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
        public static RuledHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> StyleFrom<TElm, TMgr, TArg, TBuilder, TValue>(
            this IRulable<TElm, TMgr, TArg, TBuilder, TValue> source, string style)
            where TBuilder : IViewItemHandlerBuilder<TElm, TMgr, TArg, TBuilder>
            => source.Handle().StyleFrom(style);

        public static RuledHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> StyleFrom<TElm, TMgr, TArg, TBuilder, TValue>(
            this RuledHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> source, string style)
            where TBuilder : IViewItemHandlerBuilder<TElm, TMgr, TArg, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));

            source.SubscribeOf<StyleFromContext>((value, manager, arg, ctx) =>
            {
                ctx.ReturnValue = style;
            });
            return source;
        }

        private class NameFromContext : RuleContext<string> { }
        private class StyleFromContext : RuleContext<string> { }
    }
}
