using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using R3;

namespace Lysionium.R3
{
    /// <summary>
    /// Where などの操作を許可しない Observable
    /// </summary>
    public class LUIR3HandledObservable<TElm, TMgr, TArg, TBuilder, TValue>
    {
        private readonly Observable<LUIR3Arg<TElm, TMgr, TArg, TBuilder, TValue>> observable;
        private readonly List<System.IDisposable> disposables;

        private LUIR3HandledObservable(Observable<LUIR3Arg<TElm, TMgr, TArg, TBuilder, TValue>> observable, System.IDisposable disposable)
        {
            this.observable = observable;
            disposables = new List<System.IDisposable>();
            if (disposable != null) disposables.Add(disposable);
        }

        internal static LUIR3HandledObservable<TElm, TMgr, TArg, TBuilder, TValue> Handle(
            Observable<LUIR3Arg<TElm, TMgr, TArg, TBuilder, TValue>> observable, bool handle)
        {
            observable = observable.Share();
            if (handle)
            {
                var disposable = observable.Subscribe(x => x.Info.Handle());
                return new LUIR3HandledObservable<TElm, TMgr, TArg, TBuilder, TValue>(observable, disposable);
            }
            else
            {
                return new LUIR3HandledObservable<TElm, TMgr, TArg, TBuilder, TValue>(observable, null);
            }
        }

        public void Add(System.Func<Observable<LUIR3Arg<TElm, TMgr, TArg, TBuilder, TValue>>, System.IDisposable> func)
        {
            disposables.Add(func(observable));
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
