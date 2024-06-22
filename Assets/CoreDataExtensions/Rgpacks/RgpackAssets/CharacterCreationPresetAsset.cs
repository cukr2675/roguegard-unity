using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard.Rgpacks
{
    public class CharacterCreationPresetAsset
    {
        private readonly CharacterCreationDataBuilder preset;

        public CharacterCreationPresetAsset(CharacterCreationDataBuilder preset)
        {
            this.preset = preset;
        }

        public CharacterCreationDataBuilder LoadPreset()
        {
            return new CharacterCreationDataBuilder(preset);
        }
    }
}
