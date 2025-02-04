using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF.R3
{
    public readonly struct LMFR3Arg<TElm, TMgr, TArg, TBuilder, TValue>
    {
        public TValue Value { get; }
        public TMgr Manager { get; }
        public TArg Arg { get; }
        public LMFR3Info Info { get; }

        public LMFR3Arg(TValue value, TMgr manager, TArg arg, LMFR3Info info)
        {
            Value = value;
            Manager = manager;
            Arg = arg;
            Info = info;
        }

        public LMFR3Arg<TElm, TMgr, TArg, TBuilder, T> SetValue<T>(T value)
        {
            return new LMFR3Arg<TElm, TMgr, TArg, TBuilder, T>(value, Manager, Arg, Info);
        }
    }
}
