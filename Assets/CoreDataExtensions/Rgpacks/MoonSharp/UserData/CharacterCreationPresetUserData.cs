using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Roguegard.Rgpacks.MoonSharp
{
    [MoonSharpUserData]
    [SuppressMessage("Style", "IDE1006")]
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
