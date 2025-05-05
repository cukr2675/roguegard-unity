using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium.HandlerRules
{
    /// <summary>
    /// Where などの操作を許可しない <see cref="IRulable{TElm, TMgr, TArg, TBuilder, TValue}"/>
    /// </summary>
    public class RuledHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue>
    {
        private readonly IRulable<TElm, TMgr, TArg, TBuilder, TValue> source;

        private RuledHandlerSubject(IRulable<TElm, TMgr, TArg, TBuilder, TValue> source)
        {
            this.source = source;
        }

        internal static RuledHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue> Handle(IRulable<TElm, TMgr, TArg, TBuilder, TValue> source, bool handle)
        {
            if (handle) { source.Subscribe((_, _, _, ctx) => ctx.Handle()); }
            return new RuledHandlerSubject<TElm, TMgr, TArg, TBuilder, TValue>(source);
        }

        public void SubscribeOf<TCtx>(System.Action<TValue, TMgr, TArg, TCtx> action)
            where TCtx : RuleContext
        {
            source.Subscribe((value, manager, arg, ctx) =>
            {
                if (ctx is TCtx tCtx)
                {
                    action(value, manager, arg, tCtx);
                }
            });
        }
    }
}
