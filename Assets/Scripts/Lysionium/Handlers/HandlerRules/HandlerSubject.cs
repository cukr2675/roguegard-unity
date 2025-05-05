using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium.HandlerRules
{
    public class HandlerSubject<TElm, TMgr, TArg, TOut> : IRulable<TElm, TMgr, TArg, TOut, TElm>
    {
        private readonly LinkedList<IRule<TElm, TMgr, TArg, TOut, TElm>> rules = new();

        public void Subscribe(IRule<TElm, TMgr, TArg, TOut, TElm> rule)
        {
            rules.AddLast(rule);
        }

        public void OnNext(TElm element, TMgr manager, TArg arg, RuleContext ctx)
        {
            for (var node = rules.First; node != null; node = node.Next)
            {
                node.Value.OnNext(element, manager, arg, ctx);
            }
        }
    }
}
