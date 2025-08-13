using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Roguegard.Rgpacks.MoonSharp
{
    [MoonSharpUserData]
    [SuppressMessage("Style", "IDE1006")]
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
