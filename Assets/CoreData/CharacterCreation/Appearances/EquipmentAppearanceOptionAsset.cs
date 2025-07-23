using OchalikeSprites;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    /// <summary>
    /// 装備品をアイテムではなく見た目として扱うクラス。
    /// <see cref="EquipKeywordAsset.Order"/> の影響を受けないので、付与する順番に気を付ける。
    /// </summary>
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Appearance/Singleton/Equipment")]
    [Objforming.Referable]
    public class EquipmentAppearanceOptionAsset : AppearanceOptionAsset
    {
        public override BoneKeyword BoneName => BoneKeyword.Free;

        public override Spanning<IMemberSource> MemberSources => _sources;
        private static readonly IMemberSource[] _sources = new IMemberSource[] { SingleItemMember.SourceInstance };

        public override void Affect(
            OchalikeBone mainBone, AppearanceMorph morph, IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData)
        {
            var member = SingleItemMember.GetMember(appearance);
            if (member.ItemOption is EquipmentCreationDataAsset itemData)
            {
                itemData.Affect(morph, appearance.Color);
            }
            else if (
                member.ItemOption is ObjStartingItemOption objData &&
                objData.InfoSet is SewedEquipmentInfoSet sewedInfoSet)
            {
                var data = sewedInfoSet.GetDataClone();
                data.Affect(morph, appearance.Color);
            }
        }
    }
}
