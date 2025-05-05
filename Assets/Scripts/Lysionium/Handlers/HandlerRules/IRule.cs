using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium.HandlerRules
{
    public interface IRule<TElm, TMgr, TArg, TBuilder, TValue>
    {
        void OnNext(TValue value, TMgr manager, TArg arg, RuleContext ctx);
    }
}
