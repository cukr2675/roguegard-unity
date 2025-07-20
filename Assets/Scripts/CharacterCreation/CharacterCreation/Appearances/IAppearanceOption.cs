using OchalikeSprites;

namespace Roguegard.CharacterCreation
{
    [Objforming.RequireRelationalComponent]
    public interface IAppearanceOption : IRogueDescription
    {
        /// <summary>
        /// この <see cref="IAppearanceOption"/> の前提となる <see cref="OchalikeBone"/> の名前を取得する。
        /// null のときは自由枠とする。
        /// </summary>
        BoneKeyword BoneName { get; }

        Spanning<IMemberSource> MemberSources { get; }

        void UpdateMemberRange(IMember member, IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData);

        void Affect(
            OchalikeBone mainBone, AppearanceMorph morph, IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData);
    }
}
