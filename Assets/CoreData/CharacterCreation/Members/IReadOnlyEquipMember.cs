namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyEquipMember : IReadOnlyMember
    {
        bool IsEquipped { get; }
    }
}
