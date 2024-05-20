using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace Roguegard.Scripting.MoonSharp
{
    [MoonSharpUserData]
    public class CharacterCreationAsset
    {
        private readonly KyarakuriFigurineInfo info;

        public CharacterCreationAsset(KyarakuriFigurineInfo info)
        {
            this.info = info;
        }

        public AnonWrapper<RogueObj> createObj()
        {
            var obj = info.Main.CreateObj(null, Vector2Int.zero, RogueRandom.Primary);
            return new AnonWrapper<RogueObj>(obj);
        }
    }
}
