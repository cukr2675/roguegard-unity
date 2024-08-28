using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDSSprite
{
    [CreateAssetMenu(menuName = "SDSkeletalSprite/EffecterBoneSpriteTable")]
    public class EffecterBoneSpriteTableData : ScriptableObject
    {
        [SerializeField] private Item[] _items = null;

        public void AddTo(EffectableBoneSpriteTable table)
        {
            foreach (var item in _items)
            {
                if (item.HasFirstSprite)
                {
                    if (item.OverridesSourceColor) { table.SetFirstSprite(item.Name, item.FirstSprite, item.FirstColor, item.OverridesBaseColor); }
                    else { table.SetFirstSprite(item.Name, item.FirstSprite, item.OverridesBaseColor); }
                }
                else
                {
                    if (item.OverridesSourceColor) { table.SetFirstSprite(item.Name, item.FirstColor, item.OverridesBaseColor); }
                }

                for (int i = 0; i < item.EquipmentSprites.Length; i++)
                {
                    var equipmentPair = item.EquipmentSprites[i];
                    table.AddEquipmentSprite(item.Name, equipmentPair.Sprite, equipmentPair.Color, item.OverridesBaseColor);
                }
            }
        }

        public void ColoredAddTo(EffectableBoneSpriteTable table, Color toColor)
        {
            foreach (var item in _items)
            {
                if (item.HasFirstSprite)
                {
                    if (item.OverridesSourceColor) { table.SetFirstSprite(item.Name, item.FirstSprite, item.GetColor(toColor), item.OverridesBaseColor); }
                    else { table.SetFirstSprite(item.Name, item.FirstSprite, item.OverridesBaseColor); }
                }
                else
                {
                    if (item.OverridesSourceColor) { table.SetFirstSprite(item.Name, item.GetColor(toColor), item.OverridesBaseColor); }
                }

                for (int i = 0; i < item.EquipmentSprites.Length; i++)
                {
                    var equipmentPair = item.EquipmentSprites[i];
                    table.AddEquipmentSprite(item.Name, equipmentPair.Sprite, equipmentPair.GetColor(toColor), item.OverridesBaseColor);
                }
            }
        }

        [System.Serializable]
        private class Item
        {
            [SerializeField] private BoneKeywordData _name;
            public BoneKeyword Name => _name;

            [SerializeField] private bool _hasFirstSprite;
            public bool HasFirstSprite => _hasFirstSprite;

            [SerializeField] private BoneSprite _firstSprite;
            public BoneSprite FirstSprite => _firstSprite;

            [SerializeField] private Color _firstColor;
            public Color FirstColor => _firstColor;

            [SerializeField] private bool _firstColorIsFixed;
            public bool FirstColorIsFixed => _firstColorIsFixed;

            [SerializeField] private bool _overridesSourceColor;
            public bool OverridesSourceColor => _overridesSourceColor;

            [SerializeField] private bool _overridesBaseColor;
            public bool OverridesBaseColor => _overridesBaseColor;

            [SerializeField] private BoneSpriteColorPair[] _equipmentSprites;
            public System.ReadOnlySpan<BoneSpriteColorPair> EquipmentSprites => _equipmentSprites;

            public Color GetColor(Color toColor)
            {
                if (_firstColorIsFixed) return _firstColor;
                else return toColor;
            }
        }

        [System.Serializable]
        private class BoneSpriteColorPair
        {
            [SerializeField] private BoneSprite _sprite;
            public BoneSprite Sprite => _sprite;

            [SerializeField] private Color _color;
            public Color Color => _color;

            [SerializeField] private bool _colorIsFixed;
            public bool ColorIsFixed => _colorIsFixed;

            public Color GetColor(Color toColor)
            {
                if (_colorIsFixed) return _color;
                else return toColor;
            }
        }
    }
}
