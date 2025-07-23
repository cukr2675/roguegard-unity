namespace Roguegard.CharacterCreation
{
    [Objforming.RequireRelationalComponent]
    public interface IIntrinsicOption : IRogueDescribable
    {
        Spanning<IMemberSource> MemberSources { get; }

        void UpdateMemberRange(IMember member, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData);

        int GetLv(IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData);

        float GetCost(IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, out bool costIsUnknown);

        ISortedIntrinsic CreateSortedIntrinsic(IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData);
    }
}
