using OchalikeSprites;

namespace Roguegard.CharacterCreation
{
    public abstract class AppearanceOptionAsset : RogueDescribableAsset, IAppearanceOption
    {
        /// <summary>
        /// この <see cref="AppearanceOptionAsset"/> の前提となる <see cref="OchalikeBone"/> の名前を取得する。
        /// null のときは自由枠とする。
        /// </summary>
        public abstract BoneKeyword BoneName { get; }

        public virtual Spanning<IMemberSource> MemberSources => Spanning<IMemberSource>.Empty;

        public virtual void UpdateMemberRange(IMember member, IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData)
        {
        }

        public abstract void Affect(
            OchalikeBone mainBone, AppearanceMorph morph, IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData);
    }
}
