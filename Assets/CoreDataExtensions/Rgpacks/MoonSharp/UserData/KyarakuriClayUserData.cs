using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace Roguegard.Rgpacks.MoonSharp
{
    [MoonSharpUserData]
    public class KyarakuriClayUserData
    {
        private readonly KyarakuriClayReference reference;

        public KyarakuriClayUserData(KyarakuriClayReference reference)
        {
            this.reference = reference;
        }

        public AnonWrapper<RogueObj> createObj()
        {
            var obj = reference.CreateObj(null, Vector2Int.zero, RogueRandom.Primary);
            return new AnonWrapper<RogueObj>(obj);
        }
    }
}
