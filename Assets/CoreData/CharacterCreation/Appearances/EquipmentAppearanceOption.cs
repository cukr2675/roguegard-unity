using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    /// <summary>
    /// 装備品をアイテムではなく見た目として扱うクラス。
    /// <see cref="EquipKeywordData.Order"/> の影響を受けないので、付与する順番に気を付ける。
    /// </summary>
    [CreateAssetMenu(menuName = "RoguegardData/CharacterCreation/AppearanceOption/Singleton/Equipment")]
    [ObjectFormer.Referable]
    public class EquipmentAppearanceOption : AppearanceOption
    {
        public override IKeyword BoneName => null;

        public override Spanning<IMemberSource> MemberSources => _sources;
        private static readonly IMemberSource[] _sources = new IMemberSource[] { SingleItemMember.SourceInstance };

        public override void Affect(
            BoneNodeBuilder mainNode, AppearanceBoneSpriteTable boneSpriteTable,
            IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData)
        {
            var member = SingleItemMember.GetMember(appearance);
            if (member.ItemOption is EquipmentCreationData itemData)
            {
                itemData.Affect(boneSpriteTable, appearance.Color);
            }
            else if (
                member.ItemOption is ObjStartingItemOption objData &&
                objData.InfoSet is SewedEquipmentInfoSet sewedInfoSet)
            {
                var data = sewedInfoSet.GetDataClone();
                data.Affect(boneSpriteTable, appearance.Color);
            }
        }
    }
}
