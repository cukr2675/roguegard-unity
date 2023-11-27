using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using Roguegard;
using Roguegard.CharacterCreation;

namespace RoguegardUnity
{
    public class CharacterCreationDatabase
    {
        private List<PresetCreationData> _presets = new List<PresetCreationData>();
        public Spanning<PresetCreationData> Presets => _presets;

        private List<IAppearanceOption> _appearanceOptions = new List<IAppearanceOption>();
        public Spanning<IAppearanceOption> AppearanceOptions => _appearanceOptions;

        private List<IIntrinsicOption> _intrinsicOptions = new List<IIntrinsicOption>();
        public Spanning<IIntrinsicOption> IntrinsicOptions => _intrinsicOptions;

        public void AddPreset(PresetCreationData preset)
        {
            _presets.Add(preset);
        }

        public void AddAppearanceOption(IAppearanceOption appearanceOption)
        {
            _appearanceOptions.Add(appearanceOption);
        }

        public void AddIntrinsicOption(IIntrinsicOption intrinsicOption)
        {
            _intrinsicOptions.Add(intrinsicOption);
        }
    }
}
