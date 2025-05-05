using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium.HandlerRules
{
    internal class Select<TElm, TMgr, TArg, TBuilder, TInValue, TOutValue> : IRulable<TElm, TMgr, TArg, TBuilder, TOutValue>
    {
        private readonly IRulable<TElm, TMgr, TArg, TBuilder, TInValue> source;

        private readonly Selector selector;

        public delegate TOutValue Selector(TInValue value, TMgr manager, TArg arg, RuleContext ctx);

        public Select(IRulable<TElm, TMgr, TArg, TBuilder, TInValue> source, Selector selector)
        {
            this.source = source;
            this.selector = selector;
        }

        public void Subscribe(IRule<TElm, TMgr, TArg, TBuilder, TOutValue> rule)
        {
            source.Subscribe(new Rule() { rule = rule, selector = selector });
        }

        private class Rule : IRule<TElm, TMgr, TArg, TBuilder, TInValue>
        {
            public IRule<TElm, TMgr, TArg, TBuilder, TOutValue> rule;

            public Selector selector;

            public void OnNext(TInValue inValue, TMgr manager, TArg arg, RuleContext ctx)
            {
                var outValue = selector(inValue, manager, arg, ctx);
                rule.OnNext(outValue, manager, arg, ctx);
            }
        }
    }
}
