using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace Roguegard.Scripting.MoonSharp
{
    [MoonSharpUserData]
    public class MysteryDioramaAsset
    {
        public MysteryDioramaInfo Info { get; }

        public MysteryDioramaAsset(MysteryDioramaInfo info)
        {
            Info = info;
        }

        //public AnonWrapper<RogueObj> startDungeon()
        //{
        //    var obj = info.Main.CreateObj(null, Vector2Int.zero, RogueRandom.Primary);
        //    return new AnonWrapper<RogueObj>(obj);
        //}
    }
}
