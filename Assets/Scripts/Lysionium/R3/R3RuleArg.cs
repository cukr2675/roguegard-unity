using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium.R3
{
    public readonly struct R3RuleArg<TElm, TMgr, TArg, TBuilder, TValue>
    {
        public TValue Value { get; }
        public TMgr Manager { get; }
        public TArg Arg { get; }
        public R3RuleContext Ctx { get; }

        public R3RuleArg(TValue value, TMgr manager, TArg arg, R3RuleContext ctx)
        {
            Value = value;
            Manager = manager;
            Arg = arg;
            Ctx = ctx;
        }

        public R3RuleArg<TElm, TMgr, TArg, TBuilder, T> SetValue<T>(T value)
        {
            return new R3RuleArg<TElm, TMgr, TArg, TBuilder, T>(value, Manager, Arg, Ctx);
        }
    }
}
