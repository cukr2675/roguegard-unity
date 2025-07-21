namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyQuestMember : IReadOnlyMember
    {
        Spanning<IWeightedRogueObjGenerator> Targets { get; }
        int TargetFloor { get; }
    }
}
