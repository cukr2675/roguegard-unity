using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoonSharp.Interpreter;

namespace Roguegard.Rgpacks.MoonSharp
{
    [MoonSharpUserData]
    public class MysteryDioramaUserData
    {
        public MysteryDioramaAsset Asset { get; }

        public MysteryDioramaUserData(MysteryDioramaAsset asset)
        {
            Asset = asset;
        }

        //public AnonWrapper<RogueObj> startDungeon()
        //{
        //    var obj = info.Main.CreateObj(null, Vector2Int.zero, RogueRandom.Primary);
        //    return new AnonWrapper<RogueObj>(obj);
        //}
    }
}
