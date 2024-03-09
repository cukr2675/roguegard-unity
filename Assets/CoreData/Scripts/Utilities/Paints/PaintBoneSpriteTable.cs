using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RuntimeDotter;

namespace Roguegard
{
    public class PaintBoneSpriteTable
    {
        private List<IPaintBoneSprite> _items = new List<IPaintBoneSprite>();
        public Spanning<IPaintBoneSprite> Items => _items;

        public Color32 MainColor { get; set; }

        private readonly ShiftableColor[] _palette;
        public Spanning<ShiftableColor> Palette => _palette;

        public PaintBoneSpriteTable()
        {
            _palette = new ShiftableColor[RoguegardSettings.DefaultPalette.Count];
        }

        public PaintBoneSpriteTable(PaintBoneSpriteTable table)
        {
            foreach (var item in table._items)
            {
                _items.Add(item.Clone());
            }
            MainColor = table.MainColor;
            _palette = new ShiftableColor[_palette.Length];
            for (int i = 0; i < _palette.Length; i++)
            {
                _palette[i] = table._palette[i];
            }
        }

        public void Add(IPaintBoneSprite item)
        {
            _items.Add(item);
        }

        public int IndexOf(IPaintBoneSprite item)
        {
            return _items.IndexOf(item);
        }

        public void SetPalette(int index, ShiftableColor color)
        {
            _palette[index] = color;
        }

        public AffectableBoneSpriteTable GetAffectableTable()
        {
            var table = new AffectableBoneSpriteTable();
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
