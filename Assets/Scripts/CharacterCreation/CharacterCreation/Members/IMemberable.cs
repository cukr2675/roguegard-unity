namespace Roguegard.CharacterCreation
{
    public interface IMemberable
    {
        Spanning<IMemberSource> MemberSources { get; }

        IReadOnlyMember GetMember(IMemberSource source);
    }
}
