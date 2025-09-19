using OchalikeSprites;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Appearance/Expressive Eye")]
    [Objforming.Referable]
    public class ExpressiveEyeAppearanceOptionAsset : ColoredAppearanceOptionAsset
    {
        [SerializeField] private List<ColorRangedBoneSprite> _neutralTable = null;
        public List<ColorRangedBoneSprite> NeutralTable { get => _neutralTable; set => _neutralTable = value; }

        [SerializeField] private List<ColorRangedBoneSprite> _angryTable = null;
        public List<ColorRangedBoneSprite> AngryTable { get => _angryTable; set => _angryTable = value; }

        [SerializeField] private List<ColorRangedBoneSprite> _cryingTable = null;
        public List<ColorRangedBoneSprite> CryingTable { get => _cryingTable; set => _cryingTable = value; }

        [SerializeField] private List<ColorRangedBoneSprite> _droopyTable = null;
        public List<ColorRangedBoneSprite> DroopyTable { get => _droopyTable; set => _droopyTable = value; }

        [SerializeField] private List<ColorRangedBoneSprite> _scornfulTable = null;
        public List<ColorRangedBoneSprite> ScornfulTable { get => _scornfulTable; set => _scornfulTable = value; }

        public override Spanning<IMemberSource> MemberSources => _sources;
        private static readonly IMemberSource[] _sources = new IMemberSource[] { AlphabetTypeMember.SourceInstance, ExpressiveEyeMember.SourceInstance };

        public override void UpdateMemberRange(IMember member, IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData)
        {
            if (member.Source == MemberSources[0] &&
                member is AlphabetTypeMember typeMember)
            {
                var hairColor = RogueColorUtility.GetHairColor(characterCreationData);
                var useDarkOutline = OchalikeSpritesUtility.IsSimilarToLightOutline(hairColor);
                typeMember.ClearTypeItems();
                foreach (var item in NeutralTable)
                {
                    var sprite = item.GetSprite(useDarkOutline).GetRepresentativeSprite();
                    typeMember.AddTypeItem(sprite);
                }
            }
            if (member.Source == MemberSources[1] &&
                member is ExpressiveEyeMember expressiveEyeMember)
            {
                var hairColor = RogueColorUtility.GetHairColor(characterCreationData);
                var useDarkOutline = OchalikeSpritesUtility.IsSimilarToLightOutline(hairColor);
                expressiveEyeMember.ClearTypeItems();
                if (NeutralTable.Count >= 1)
                {
                    var sprite = NeutralTable[0].GetSprite(useDarkOutline).GetRepresentativeSprite();
                    expressiveEyeMember.AddTypeItem(ExpressiveEyeType.Neutral, sprite);
                }
                if (AngryTable.Count >= 1)
                {
                    var sprite = AngryTable[0].GetSprite(useDarkOutline).GetRepresentativeSprite();
                    expressiveEyeMember.AddTypeItem(ExpressiveEyeType.Angry, sprite);
                }
                if (CryingTable.Count >= 1)
                {
                    var sprite = CryingTable[0].GetSprite(useDarkOutline).GetRepresentativeSprite();
                    expressiveEyeMember.AddTypeItem(ExpressiveEyeType.Crying, sprite);
                }
                if (DroopyTable.Count >= 1)
                {
                    var sprite = DroopyTable[0].GetSprite(useDarkOutline).GetRepresentativeSprite();
                    expressiveEyeMember.AddTypeItem(ExpressiveEyeType.Droopy, sprite);
                }
                if (ScornfulTable.Count >= 1)
                {
                    var sprite = ScornfulTable[0].GetSprite(useDarkOutline).GetRepresentativeSprite();
                    expressiveEyeMember.AddTypeItem(ExpressiveEyeType.Scornful, sprite);
                }
            }
        }

        protected override BoneSprite GetSprite(IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData)
        {
            var hairColor = RogueColorUtility.GetHairColor(characterCreationData);
            var useDarkOutline = OchalikeSpritesUtility.IsSimilarToLightOutline(hairColor);
            var expressiveEyeMember = ExpressiveEyeMember.GetMember(appearance);
            var table = expressiveEyeMember.Type switch
            {
                ExpressiveEyeType.Neutral => NeutralTable,
                ExpressiveEyeType.Angry => AngryTable,
                ExpressiveEyeType.Crying => CryingTable,
                ExpressiveEyeType.Droopy => DroopyTable,
                ExpressiveEyeType.Scornful => ScornfulTable,
                _ => NeutralTable,
            };
            var typeMember = AlphabetTypeMember.GetMember(appearance);
            var typeIndex = Mathf.Clamp(typeMember.TypeIndex, 0, table.Count);
            var sprite = table[typeIndex].GetSprite(useDarkOutline);
            return sprite;
        }
    }
}
