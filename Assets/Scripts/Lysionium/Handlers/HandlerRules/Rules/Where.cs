using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium.HandlerRules
{
    internal class Where<TElm, TMgr, TArg, TBuilder, TValue> : IRulable<TElm, TMgr, TArg, TBuilder, TValue>
    {
        private readonly IRulable<TElm, TMgr, TArg, TBuilder, TValue> source;

        private readonly Predicator predicate;

        public delegate bool Predicator(TValue value, TMgr manager, TArg arg, RuleContext ctx);

        public Where(IRulable<TElm, TMgr, TArg, TBuilder, TValue> source, Predicator predicate)
        {
            this.source = source;
            this.predicate = predicate;
        }

        public void Subscribe(IRule<TElm, TMgr, TArg, TBuilder, TValue> rule)
        {
            source.Subscribe(new Rule() { rule = rule, predicate = predicate });
        }

        private class Rule : IRule<TElm, TMgr, TArg, TBuilder, TValue>
        {
            public IRule<TElm, TMgr, TArg, TBuilder, TValue> rule;

            public Predicator predicate;

            public void OnNext(TValue value, TMgr manager, TArg arg, RuleContext ctx)
            {
                if (predicate(value, manager, arg, ctx))
                {
                    rule.OnNext(value, manager, arg, ctx);
                }
            }
        }
    }
}
