using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using R3;

namespace Lysionium.R3
{
    /// <summary>
    /// Where などの操作を許可しない Observable
    /// </summary>
    public class ObservedHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue>
    {
        private readonly Observable<R3RuleArg<TElm, TMgr, TArg, TBuilder, TValue>> source;
        private readonly List<System.IDisposable> disposables;

        private ObservedHandlerSubject(Observable<R3RuleArg<TElm, TMgr, TArg, TBuilder, TValue>> source, System.IDisposable disposable)
        {
            this.source = source;
            disposables = new List<System.IDisposable>();
            if (disposable != null) { disposables.Add(disposable); }
        }

        internal static ObservedHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> Handle(
            Observable<R3RuleArg<TElm, TMgr, TArg, TBuilder, TValue>> source, bool handle)
        {
            source = source.Share();
            if (handle)
            {
                var disposable = source.Subscribe(x => x.Ctx.Handle());
                return new ObservedHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue>(source, disposable);
            }
            else
            {
                return new ObservedHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue>(source, null);
            }
        }

        public void SubscribeOf<TCtx>(System.Action<TValue, TMgr, TArg, TCtx> action)
            where TCtx : R3RuleContext
        {
            disposables.Add(
                source.Subscribe(x =>
                {
                    if (x.Ctx is TCtx tCtx)
                    {
                        action(x.Value, x.Manager, x.Arg, tCtx);
                    }
                }));
        }

        public void Dispose()
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
