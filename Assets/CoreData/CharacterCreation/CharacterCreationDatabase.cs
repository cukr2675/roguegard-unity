using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class CharacterCreationDatabase : ICharacterCreationDatabase
    {
        private List<CharacterCreationDataBuilder> _presets = new List<CharacterCreationDataBuilder>();
        public int PresetsCount => _presets.Count;

        private List<IAppearanceOption> _appearanceOptions = new List<IAppearanceOption>();
        public Spanning<IAppearanceOption> AppearanceOptions => _appearanceOptions;

        private List<IIntrinsicOption> _intrinsicOptions = new List<IIntrinsicOption>();
        public Spanning<IIntrinsicOption> IntrinsicOptions => _intrinsicOptions;

        private List<IStartingItemOption> _startingItemOptions = new List<IStartingItemOption>();
        public Spanning<IStartingItemOption> StartingItemOptions => _startingItemOptions;

        public CharacterCreationDataBuilder LoadPreset(int index)
        {
            return new CharacterCreationDataBuilder(_presets[index]);
        }

        public void AddPreset(CharacterCreationDataBuilder preset)
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

        public void AddStartingItemOption(IStartingItemOption startingItemOption)
        {
            _startingItemOptions.Add(startingItemOption);
        }
    }
}
