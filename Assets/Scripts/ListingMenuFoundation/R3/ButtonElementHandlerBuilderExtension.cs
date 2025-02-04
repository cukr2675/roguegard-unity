using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using R3;

namespace ListingMF.R3
{
    public static class ButtonElementHandlerBuilderExtension
    {
        public static TOut SubscribeButtonElementHandler<TElm, TMgr, TArg, TOut>(
            this IButtonElementHandlerBuilder<TElm, TMgr, TArg, TOut> builder, Subject<LMFR3Arg<TElm, TMgr, TArg, TOut, TElm>> subject)
        {
            if (builder == null) throw new System.ArgumentNullException(nameof(builder));
            if (subject == null) throw new System.ArgumentNullException(nameof(subject));
            
            var onClickInfo = new OnClickInfo();
            builder.OnClick((element, manager, arg) =>
            {
                using var _ = onClickInfo.OpenSelf();
                subject.OnNext(new LMFR3Arg<TElm, TMgr, TArg, TOut, TElm>(element, manager, arg, onClickInfo));
            });

            return (TOut)builder;
        }

        /// <summary>
        /// 暗黙の Handle() 呼び出し
        /// </summary>
        public static LMFR3HandledObservable<TElm, TMgr, TArg, TBuilder, TValue> OnClick<TElm, TMgr, TArg, TBuilder, TValue>(
            this Observable<LMFR3Arg<TElm, TMgr, TArg, TBuilder, TValue>> observable, HandleClickElement<TValue, TMgr, TArg> onClick)
            where TBuilder : IButtonElementHandlerBuilder<TElm, TMgr, TArg, TBuilder>
            => observable.Handle().OnClick(onClick);

        public static LMFR3HandledObservable<TElm, TMgr, TArg, TBuilder, TValue> OnClick<TElm, TMgr, TArg, TBuilder, TValue>(
            this LMFR3HandledObservable<TElm, TMgr, TArg, TBuilder, TValue> observable, HandleClickElement<TValue, TMgr, TArg> onClick)
            where TBuilder : IButtonElementHandlerBuilder<TElm, TMgr, TArg, TBuilder>
        {
            if (observable == null) throw new System.ArgumentNullException(nameof(observable));
            if (onClick == null) throw new System.ArgumentNullException(nameof(onClick));

            observable.Add(
                _ => _
                .Where(x => x.Info is OnClickInfo)
                .Subscribe(x =>
                {
                    onClick(x.Value, x.Manager, x.Arg);
                }));
            return observable;
        }

        private class OnClickInfo : LMFR3Info { }
    }
}
