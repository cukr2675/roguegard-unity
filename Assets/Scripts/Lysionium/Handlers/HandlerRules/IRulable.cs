using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium.HandlerRules
{
    public interface IRulable<TElm, TMgr, TArg, TBuilder, TValue>
    {
        void Subscribe(IRule<TElm, TMgr, TArg, TBuilder, TValue> rule);

        public sealed void Subscribe(System.Action<TValue, TMgr, TArg, RuleContext> onNext)
        {
            Subscribe(new ActionRule<TElm, TMgr, TArg, TBuilder, TValue>(onNext));
        }
    }
}
