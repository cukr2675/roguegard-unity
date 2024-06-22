using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace Roguegard.Rgpacks.MoonSharp
{
    [MoonSharpUserData]
    public class CharacterCreationPresetUserData
    {
        private readonly CharacterCreationPresetAsset asset;

        public CharacterCreationPresetUserData(CharacterCreationPresetAsset asset)
        {
            this.asset = asset;
        }

        public AnonWrapper<RogueObj> createObj()
        {
            var data = asset.LoadPreset();
            var obj = data.CreateObj(null, Vector2Int.zero, RogueRandom.Primary);
            return new AnonWrapper<RogueObj>(obj);
        }
    }
}
