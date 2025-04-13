using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium.R3
{
    public readonly struct LUIR3Arg<TElm, TMgr, TArg, TBuilder, TValue>
    {
        public TValue Value { get; }
        public TMgr Manager { get; }
        public TArg Arg { get; }
        public LUIR3Info Info { get; }

        public LUIR3Arg(TValue value, TMgr manager, TArg arg, LUIR3Info info)
        {
            Value = value;
            Manager = manager;
            Arg = arg;
            Info = info;
        }

        public LUIR3Arg<TElm, TMgr, TArg, TBuilder, T> SetValue<T>(T value)
        {
            return new LUIR3Arg<TElm, TMgr, TArg, TBuilder, T>(value, Manager, Arg, Info);
        }
    }
}
