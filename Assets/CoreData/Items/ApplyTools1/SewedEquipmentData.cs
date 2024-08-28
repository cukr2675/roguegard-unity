using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using Roguegard.CharacterCreation;

namespace Roguegard
{
    [Objforming.Formable]
    public class SewedEquipmentData
    {
        public string Name { get; set; }

        private ISerializableKeyword[] _equipParts;
        public Spanning<IKeyword> EquipParts => _equipParts;

        public float BoneSpriteEffectOrder { get; set; }

        public PaintBoneSpriteTable BoneSprites { get; }

        [System.NonSerialized] private Sprite _icon;
        public Sprite Icon
        {
            get
            {
                if (_icon == null) { _icon = BoneSprites.GetIcon(); }
                return _icon;
            }
        }

        public SewedEquipmentData()
        {
            Name = "";
            _equipParts = new ISerializableKeyword[0];
            BoneSpriteEffectOrder = 0;
            BoneSprites = new PaintBoneSpriteTable();
        }

        public SewedEquipmentData(SewedEquipmentData data)
        {
            Name = data.Name;
            _equipParts = data._equipParts?.ToArray();
            BoneSpriteEffectOrder = data.BoneSpriteEffectOrder;
            BoneSprites = new PaintBoneSpriteTable(data.BoneSprites);
        }

        public void SetEquipParts(Spanning<ISerializableKeyword> parts)
        {
            _equipParts = parts.ToArray();
        }

        public void Affect(AppearanceBoneSpriteTable boneSpriteTable, Color color)
        {
            if (!boneSpriteTable.TryGetNewEquipmentTable(_equipParts, BoneSpriteEffectOrder, out var table))
            {
                Debug.LogWarning("èdï°ÇµÇΩëïîıïîà ÇÃå©ÇΩñ⁄Ç™ë∂ç›ÇµÇ‹Ç∑ÅB");
                return;
            }

            BoneSprites.GetEffectableTable().ColoredAddTo(table, color);
        }
    }
}
