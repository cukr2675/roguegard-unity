using Roguegard.CharacterCreation;
using System.Linq;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class SewedEquipmentData
    {
        public string Name { get; set; }

        private ISerializableKeyword[] _equipmentSlots;
        public Spanning<IKeyword> EquipmentSlots => _equipmentSlots;

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
            _equipmentSlots = new ISerializableKeyword[0];
            BoneSpriteEffectOrder = 0;
            BoneSprites = new PaintBoneSpriteTable();
        }

        public SewedEquipmentData(SewedEquipmentData data)
        {
            Name = data.Name;
            _equipmentSlots = data._equipmentSlots?.ToArray();
            BoneSpriteEffectOrder = data.BoneSpriteEffectOrder;
            BoneSprites = new PaintBoneSpriteTable(data.BoneSprites);
        }

        public void SetEquipmentSlots(Spanning<ISerializableKeyword> equipmentSlots)
        {
            _equipmentSlots = equipmentSlots.ToArray();
        }

        public void Affect(AppearanceMorph morph, Color color)
        {
            if (!morph.TryGetNewEquipmentTable(_equipmentSlots, BoneSpriteEffectOrder, out var table))
            {
                Debug.LogWarning("重複した装備部位の見た目が存在します。");
                return;
            }

            BoneSprites.GetOchalikeMorph().ColoredAddTo(table, color);
        }
    }
}
