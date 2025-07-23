namespace Roguegard.CharacterCreation
{
    public interface IReadOnlyMemberable
    {
        Spanning<IMemberSource> MemberSources { get; }

        IReadOnlyMember GetMember(IMemberSource source);
    }
}
