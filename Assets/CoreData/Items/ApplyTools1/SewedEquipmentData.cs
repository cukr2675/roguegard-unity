using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class SewedEquipmentData
    {
        public string Name { get; set; }

        public Color32 MainColor { get; set; }

        private ISerializableKeyword[] _equipParts;
        public Spanning<IKeyword> EquipParts
        {
            get => _equipParts;
            set
            {
                _equipParts = new ISerializableKeyword[value.Count];
                for (int i = 0; i < value.Count; i++)
                {
                    _equipParts[i] = (ISerializableKeyword)value[i];
                }
            }
        }

        public float BoneSpriteEffectOrder { get; set; }

        public SewedEquipmentDataItemTable Items { get; }

        private readonly RoguePaintColor[] _palette;
        public Spanning<RoguePaintColor> Palette => _palette;

        [System.NonSerialized] private Sprite _icon;
        public Sprite Icon
        {
            get
            {
                if (_icon == null) { _icon = Items.GetIcon(_palette); }
                return _icon;
            }
        }

        public SewedEquipmentData()
        {
            Items = new SewedEquipmentDataItemTable();
            _palette = new RoguePaintColor[RoguePaintData.PaletteSize];
            for (int i = 0; i < _palette.Length; i++)
            {
                _palette[i] = new RoguePaintColor();
            }
        }

        public SewedEquipmentData(SewedEquipmentData data)
        {
            Name = data.Name;
            MainColor = data.MainColor;
            _equipParts = data._equipParts?.ToArray();
            BoneSpriteEffectOrder = data.BoneSpriteEffectOrder;
            Items = new SewedEquipmentDataItemTable(data.Items);
            _palette = new RoguePaintColor[RoguePaintData.PaletteSize];
            for (int i = 0; i < _palette.Length; i++)
            {
                _palette[i] = new RoguePaintColor(data._palette[i]);
            }
        }
    }
}
