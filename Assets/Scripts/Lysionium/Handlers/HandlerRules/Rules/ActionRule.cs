using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium.HandlerRules
{
    internal class ActionRule<TElm, TMgr, TArg, TBuilder, TValue> : IRule<TElm, TMgr, TArg, TBuilder, TValue>
    {
        private readonly System.Action<TValue, TMgr, TArg, RuleContext> onNext;

        public ActionRule(System.Action<TValue, TMgr, TArg, RuleContext> onNext)
        {
            if (onNext == null) throw new System.ArgumentNullException(nameof(onNext));

            this.onNext = onNext;
        }

        public void OnNext(TValue value, TMgr manager, TArg arg, RuleContext ctx)
        {
            onNext(value, manager, arg, ctx);
        }
    }
}
