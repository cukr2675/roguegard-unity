using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;
using RuntimeDotter;

namespace Roguegard
{
    [Objforming.Formable]
    public class PaintBoneSpriteTable
    {
        private readonly List<IPaintBoneSprite> _items = new List<IPaintBoneSprite>();
        public Spanning<IPaintBoneSprite> Items => _items;

        public Color32 MainColor { get; set; }

        private readonly ShiftableColor[] _palette;
        public Spanning<ShiftableColor> Palette => _palette;

        public PaintBoneSpriteTable()
        {
            _palette = new ShiftableColor[RoguegardSettings.DefaultPalette.Count];
        }

        [Objforming.CreateInstance]
        private PaintBoneSpriteTable(bool flag) { }

        public PaintBoneSpriteTable(PaintBoneSpriteTable table)
        {
            foreach (var item in table._items)
            {
                _items.Add(item.Clone());
            }
            MainColor = table.MainColor;
            _palette = new ShiftableColor[table._palette.Length];
            for (int i = 0; i < _palette.Length; i++)
            {
                _palette[i] = table._palette[i];
            }
        }

        public void Add(IPaintBoneSprite item)
        {
            _items.Add(item);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public int IndexOf(IPaintBoneSprite item)
        {
            return _items.IndexOf(item);
        }

        public void SetPalette(int index, ShiftableColor color)
        {
            _palette[index] = color;
        }

        public EffectableBoneSpriteTable GetEffectableTable()
        {
            var table = new EffectableBoneSpriteTable();
            foreach (var item in _items)
            {
                item.AddTo(table, MainColor, _palette);
            }
            return table;
        }

        public Sprite GetIcon()
        {
            return _items[0].GetIcon(_palette);
        }
    }
}
