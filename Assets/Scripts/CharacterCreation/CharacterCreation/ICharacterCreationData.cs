namespace Roguegard.CharacterCreation
{
    [Objforming.RequireRelationalComponent]
    public interface ICharacterCreationData : IRogueDescribable
    {
        float Cost { get; }
        bool CostIsUnknown { get; }
        IReadOnlyRace Race { get; }
        Spanning<IReadOnlyAppearance> Appearances { get; }
        ISortedIntrinsicList SortedIntrinsics { get; }
        Spanning<IWeightedRogueObjGeneratorList> StartingItemTable { get; }

        bool TryGetGrowingInfoSet(IRaceOption raceOption, IRogueGender gender, out IMainInfoSet growingInfoSet);
    }
}
