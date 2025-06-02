using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium.HandlerRules
{
    public static class ButtonElementHandlerBuilderExtension
    {
        public static TOut SubscribeButtonElementHandler<TElm, TMgr, TArg, TOut>(
            this IButtonElementHandlerBuilder<TElm, TMgr, TArg, TOut> builder, HandlerSubject<TElm, TMgr, TArg, TOut> subject)
        {
            if (builder == null) throw new System.ArgumentNullException(nameof(builder));
            if (subject == null) throw new System.ArgumentNullException(nameof(subject));
            
            var onClickCtx = new OnClickContext();
            builder.OnClick((element, manager, arg) =>
            {
                lock (onClickCtx)
                {
                    using var _ = onClickCtx.OpenSelf();
                    subject.OnNext(element, manager, arg, onClickCtx);
                }
            });

            return (TOut)builder;
        }

        /// <summary>
        /// 暗黙の Handle() 呼び出し
        /// </summary>
        public static RuledHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> OnClick<TElm, TMgr, TArg, TBuilder, TValue>(
            this IRulable<TElm, TMgr, TArg, TBuilder, TValue> source, HandleClickElement<TValue, TMgr, TArg> onClick)
            where TBuilder : IButtonElementHandlerBuilder<TElm, TMgr, TArg, TBuilder>
            => source.Handle().OnClick(onClick);

        public static RuledHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> OnClick<TElm, TMgr, TArg, TBuilder, TValue>(
            this RuledHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> source, HandleClickElement<TValue, TMgr, TArg> onClick)
            where TBuilder : IButtonElementHandlerBuilder<TElm, TMgr, TArg, TBuilder>
        {
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (onClick == null) throw new System.ArgumentNullException(nameof(onClick));

            source.SubscribeOf<OnClickContext>((value, manager, arg, _) =>
            {
                onClick(value, manager, arg);
            });
            return source;
        }

        private class OnClickContext : RuleContext { }
    }
}
