using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class CharacterCreationDatabase : ICharacterCreationDatabase
    {
        private List<CharacterCreationDataBuilder> _presets = new List<CharacterCreationDataBuilder>();
        public int PresetsCount => _presets.Count;

        private List<IRaceOption> _raceOptions = new();
        public Spanning<IRaceOption> RaceOptions => Spanning.Get(_raceOptions);

        private List<IAppearanceOption> _appearanceOptions = new();
        public Spanning<IAppearanceOption> AppearanceOptions => Spanning.Get(_appearanceOptions);

        private List<IIntrinsicOption> _intrinsicOptions = new();
        public Spanning<IIntrinsicOption> IntrinsicOptions => Spanning.Get(_intrinsicOptions);

        private List<IStartingItemOption> _startingItemOptions = new();
        public Spanning<IStartingItemOption> StartingItemOptions => Spanning.Get(_startingItemOptions);

        public CharacterCreationDataBuilder LoadPreset(int index)
        {
            return new CharacterCreationDataBuilder(_presets[index]);
        }

        public void AddPreset(CharacterCreationDataBuilder preset)
        {
            _presets.Add(preset);
        }

        public void AddRaceOptions(IRaceOption raceOptions)
        {
            _raceOptions.Add(raceOptions);
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
