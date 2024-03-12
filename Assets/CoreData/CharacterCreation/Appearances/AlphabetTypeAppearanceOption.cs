using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/AppearanceOption/Alphabet")]
    [ObjectFormer.Referable]
    public class AlphabetTypeAppearanceOption : ColoredAppearanceOption
    {
        [SerializeField] private List<ColorRangedBoneSprite> _table = null;
        public List<ColorRangedBoneSprite> Table { get => _table; set => _table = value; }

        public override Spanning<IMemberSource> MemberSources => _sources;
        private static readonly IMemberSource[] _sources = new IMemberSource[] { AlphabetTypeMember.SourceInstance };

        public override void UpdateMemberRange(IMember member, IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData)
        {
            if (member.Source == MemberSources[0] &&
                member is AlphabetTypeMember typeMember)
            {
                var bodyColor = characterCreationData.Race.BodyColor;
                var colorRange = RogueColorUtility.GetColorRange(bodyColor);
                typeMember.ClearTypeItems();
                foreach (var item in Table)
                {
                    var sprite = item.GetSprite(bodyColor).GetRepresentativeSprite();
                    typeMember.AddTypeItem(sprite);
                }
            }
        }

        protected override BoneSprite GetSprite(IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData)
        {
            var bodyColor = characterCreationData.Race.BodyColor;
            var member = AlphabetTypeMember.GetMember(appearance);
            var typeIndex = Mathf.Clamp(member.TypeIndex, 0, Table.Count);
            var colorRange = RogueColorUtility.GetColorRange(bodyColor);
            var sprite = Table[typeIndex].GetSprite(bodyColor);
            return sprite;
        }
    }
}
