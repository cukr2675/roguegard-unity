using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using R3;

namespace ListingMF.R3
{
    public static class ElementHandlerBuilderExtension
    {
        public static TOut R3<TElm, TMgr, TArg, TOut>(
            this IElementHandlerBuilder<TElm, TMgr, TArg, TOut> builder, out Subject<LMFR3Arg<TElm, TMgr, TArg, TOut, TElm>> subject)
        {
            subject = new Subject<LMFR3Arg<TElm, TMgr, TArg, TOut, TElm>>();
            SubscribeElementHandler(builder, subject);

            if (builder is IButtonElementHandlerBuilder<TElm, TMgr, TArg, TOut> buttonsBuilder)
            {
                ButtonElementHandlerBuilderExtension.SubscribeButtonElementHandler(buttonsBuilder, subject);
            }

            return (TOut)builder;
        }

        public static LMFR3HandledObservable<TElm, TMgr, TArg, TBuilder, TValue> Handle<TElm, TMgr, TArg, TBuilder, TValue>(
            this Observable<LMFR3Arg<TElm, TMgr, TArg, TBuilder, TValue>> observable)
        {
            return LMFR3HandledObservable<TElm, TMgr, TArg, TBuilder, TValue>.Handle(observable, true);
        }

        public static LMFR3HandledObservable<TElm, TMgr, TArg, TBuilder, TValue> WithoutHandle<TElm, TMgr, TArg, TBuilder, TValue>(
            this Observable<LMFR3Arg<TElm, TMgr, TArg, TBuilder, TValue>> observable)
        {
            return LMFR3HandledObservable<TElm, TMgr, TArg, TBuilder, TValue>.Handle(observable, false);
        }

        public static Observable<LMFR3Arg<TElm, TMgr, TArg, TBuilder, TValue>> NotHandled<TElm, TMgr, TArg, TBuilder, TValue>(
            this Observable<LMFR3Arg<TElm, TMgr, TArg, TBuilder, TValue>> observable)
        {
            return observable.Where(x => !x.Info.IsHandled);
        }

        internal static Observable<LMFR3Arg<TElm, TMgr, TArg, TBuilder, TValue>> Fallback<TElm, TMgr, TArg, TBuilder, TValue>(
            this Observable<LMFR3Arg<TElm, TMgr, TArg, TBuilder, TValue>> observable)
        {
            return observable.Where(x => ((LMFR3Info<object>)x.Info).ReturnValue == null);
        }

        public static Observable<LMFR3Arg<TElm, TMgr, TArg, TBuilder, TValue>> Where<TElm, TMgr, TArg, TBuilder, TValue>(
            this Observable<LMFR3Arg<TElm, TMgr, TArg, TBuilder, TValue>> observable, System.Func<TValue, TMgr, TArg, bool> predicate)
        {
            return observable.Where(x => predicate(x.Value, x.Manager, x.Arg));
        }

        public static Observable<LMFR3Arg<TElm, TMgr, TArg, TBuilder, TValue>> WhereElm<TElm, TMgr, TArg, TBuilder, TValue>(
            this Observable<LMFR3Arg<TElm, TMgr, TArg, TBuilder, TValue>> observable, System.Func<TValue, bool> predicate)
        {
            return observable.Where(x => predicate(x.Value));
        }

        public static Observable<LMFR3Arg<TElm, TMgr, TArg, TBuilder, TOutValue>> Select<TElm, TMgr, TArg, TBuilder, TInValue, TOutValue>(
            this Observable<LMFR3Arg<TElm, TMgr, TArg, TBuilder, TInValue>> observable, System.Func<TInValue, TMgr, TArg, TOutValue> selector)
        {
            return observable.Select(x =>
            {
                var value = selector(x.Value, x.Manager, x.Arg);
                return x.SetValue(value);
            });
        }

        public static Observable<LMFR3Arg<TElm, TMgr, TArg, TBuilder, TOutValue>> SelectElm<TElm, TMgr, TArg, TBuilder, TInValue, TOutValue>(
            this Observable<LMFR3Arg<TElm, TMgr, TArg, TBuilder, TInValue>> observable, System.Func<TInValue, TOutValue> selector)
        {
            return observable.Select(x =>
            {
                var value = selector(x.Value);
                return x.SetValue(value);
            });
        }



        public static TOut SubscribeElementHandler<TElm, TMgr, TArg, TOut>(
            this IElementHandlerBuilder<TElm, TMgr, TArg, TOut> builder, Subject<LMFR3Arg<TElm, TMgr, TArg, TOut, TElm>> subject)
        {
            if (builder == null) throw new System.ArgumentNullException(nameof(builder));
            if (subject == null) throw new System.ArgumentNullException(nameof(subject));

            var nameFromInfo = new NameFromInfo();
            builder.NameFrom((element, manager, arg) =>
            {
                using var _ = nameFromInfo.OpenSelf();
                subject.OnNext(new LMFR3Arg<TElm, TMgr, TArg, TOut, TElm>(element, manager, arg, nameFromInfo));
                return nameFromInfo.ReturnValue;
            });

            var styleFromInfo = new StyleFromInfo();
            builder.StyleFrom((element, manager, arg) =>
            {
                using var _ = styleFromInfo.OpenSelf();
                subject.OnNext(new LMFR3Arg<TElm, TMgr, TArg, TOut, TElm>(element, manager, arg, styleFromInfo));
                return styleFromInfo.ReturnValue;
            });

            return (TOut)builder;
        }

        /// <summary>
        /// 暗黙の Handle() 呼び出し
        /// </summary>
        public static LMFR3HandledObservable<TElm, TMgr, TArg, TBuilder, TValue> NameFrom<TElm, TMgr, TArg, TBuilder, TValue>(
            this Observable<LMFR3Arg<TElm, TMgr, TArg, TBuilder, TValue>> observable, GetElementName<TValue, TMgr, TArg> nameFrom)
            where TBuilder : IElementHandlerBuilder<TElm, TMgr, TArg, TBuilder>
            => observable.Handle().NameFrom(nameFrom);

        public static LMFR3HandledObservable<TElm, TMgr, TArg, TBuilder, TValue> NameFrom<TElm, TMgr, TArg, TBuilder, TValue>(
            this LMFR3HandledObservable<TElm, TMgr, TArg, TBuilder, TValue> observable, GetElementName<TValue, TMgr, TArg> nameFrom)
            where TBuilder : IElementHandlerBuilder<TElm, TMgr, TArg, TBuilder>
        {
            if (observable == null) throw new System.ArgumentNullException(nameof(observable));
            if (nameFrom == null) throw new System.ArgumentNullException(nameof(nameFrom));

            observable.Add(
                _ => _
                .Where(x => x.Info is NameFromInfo)
                .Subscribe(x =>
                {
                    ((NameFromInfo)x.Info).ReturnValue = nameFrom(x.Value, x.Manager, x.Arg);
                }));
            return observable;
        }

        /// <summary>
        /// 暗黙の Handle() 呼び出し
        /// </summary>
        public static LMFR3HandledObservable<TElm, TMgr, TArg, TBuilder, TValue> StyleFrom<TElm, TMgr, TArg, TBuilder, TValue>(
            this Observable<LMFR3Arg<TElm, TMgr, TArg, TBuilder, TValue>> observable, GetElementName<TValue, TMgr, TArg> styleFrom)
            where TBuilder : IElementHandlerBuilder<TElm, TMgr, TArg, TBuilder>
            => observable.Handle().StyleFrom(styleFrom);

        public static LMFR3HandledObservable<TElm, TMgr, TArg, TBuilder, TValue> StyleFrom<TElm, TMgr, TArg, TBuilder, TValue>(
            this LMFR3HandledObservable<TElm, TMgr, TArg, TBuilder, TValue> observable, GetElementName<TValue, TMgr, TArg> styleFrom)
            where TBuilder : IElementHandlerBuilder<TElm, TMgr, TArg, TBuilder>
        {
            if (observable == null) throw new System.ArgumentNullException(nameof(observable));
            if (styleFrom == null) throw new System.ArgumentNullException(nameof(styleFrom));

            observable.Add(
                _ => _
                .Where(x => x.Info is StyleFromInfo)
                .Subscribe(x =>
                {
                    ((StyleFromInfo)x.Info).ReturnValue = styleFrom(x.Value, x.Manager, x.Arg);
                }));
            return observable;
        }

        /// <summary>
        /// 暗黙の Handle() 呼び出し
        /// </summary>
        public static LMFR3HandledObservable<TElm, TMgr, TArg, TBuilder, TValue> StyleFrom<TElm, TMgr, TArg, TBuilder, TValue>(
            this Observable<LMFR3Arg<TElm, TMgr, TArg, TBuilder, TValue>> observable, string style)
            where TBuilder : IElementHandlerBuilder<TElm, TMgr, TArg, TBuilder>
            => observable.Handle().StyleFrom(style);

        public static LMFR3HandledObservable<TElm, TMgr, TArg, TBuilder, TValue> StyleFrom<TElm, TMgr, TArg, TBuilder, TValue>(
            this LMFR3HandledObservable<TElm, TMgr, TArg, TBuilder, TValue> observable, string style)
            where TBuilder : IElementHandlerBuilder<TElm, TMgr, TArg, TBuilder>
        {
            if (observable == null) throw new System.ArgumentNullException(nameof(observable));

            observable.Add(
                _ => _
                .Where(x => x.Info is StyleFromInfo)
                .Subscribe(x =>
                {
                    ((StyleFromInfo)x.Info).ReturnValue = style;
                }));
            return observable;
        }

        private class NameFromInfo : LMFR3Info<string> { }
        private class StyleFromInfo : LMFR3Info<string> { }
    }
}
