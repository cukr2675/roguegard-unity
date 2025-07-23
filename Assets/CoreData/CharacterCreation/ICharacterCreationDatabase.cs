namespace Roguegard.CharacterCreation
{
    public interface ICharacterCreationDatabase
    {
        int PresetsCount { get; }

        Spanning<IRaceOption> RaceOptions { get; }

        Spanning<IAppearanceOption> AppearanceOptions { get; }

        Spanning<IIntrinsicOption> IntrinsicOptions { get; }

        Spanning<IStartingItemOption> StartingItemOptions { get; }

        public CharacterCreationData LoadPreset(int index);
    }
}
