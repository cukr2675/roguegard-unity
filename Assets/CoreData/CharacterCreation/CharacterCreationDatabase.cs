using System.Collections.Generic;

namespace Roguegard.CharacterCreation
{
    public class CharacterCreationDatabase : ICharacterCreationDatabase
    {
        private readonly List<CharacterCreationData> _presets = new();
        public int PresetsCount => _presets.Count;

        private readonly List<IRaceOption> _raceOptions = new();
        public Spanning<IRaceOption> RaceOptions => Spanning.Get(_raceOptions);

        private readonly List<IAppearanceOption> _appearanceOptions = new();
        public Spanning<IAppearanceOption> AppearanceOptions => Spanning.Get(_appearanceOptions);

        private readonly List<IIntrinsicOption> _intrinsicOptions = new();
        public Spanning<IIntrinsicOption> IntrinsicOptions => Spanning.Get(_intrinsicOptions);

        private readonly List<IStartingItemOption> _startingItemOptions = new();
        public Spanning<IStartingItemOption> StartingItemOptions => Spanning.Get(_startingItemOptions);

        public CharacterCreationData LoadPreset(int index)
        {
            return new CharacterCreationData(_presets[index]);
        }

        public void AddPreset(CharacterCreationData preset)
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
