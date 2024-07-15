using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "RoguegardData/Sprite/EffectableBoneSpriteTable")]
    public class EffectableBoneSpriteTableData : ScriptableObject
    {
        [SerializeField] private Value _value = null;

        public EffectableBoneSpriteTable Table => _value.Table;

        [System.Serializable]
        public class Value
        {
            [SerializeField] private List<Item> _items;

            [System.NonSerialized] private EffectableBoneSpriteTable _table;
            public EffectableBoneSpriteTable Table
            {
                get
                {
                    if (_table == null) Enable();

                    return _table;
                }
            }

            private void Enable()
            {
                _table = new EffectableBoneSpriteTable();
                foreach (var item in _items)
                {
                    if (item.HasFirstSprite)
                    {
                        if (item.OverridesSourceColor) { Table.SetFirstSprite(item.Name, item.FirstSprite, item.FirstColor, item.OverridesBaseColor); }
                        else { Table.SetFirstSprite(item.Name, item.FirstSprite, item.OverridesBaseColor); }
                    }
                    for (int i = 0; i < item.EquipmentSprites.Count; i++)
                    {
                        var equipmentPair = item.EquipmentSprites[i];
                        Table.AddEquipmentSprite(item.Name, equipmentPair.Sprite, equipmentPair.Color, item.OverridesBaseColor);
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

                [SerializeField] private bool _overridesSourceColor;
                public bool OverridesSourceColor => _overridesSourceColor;

                [SerializeField] private bool _overridesBaseColor;
                public bool OverridesBaseColor => _overridesBaseColor;

                [SerializeField] private List<BoneSpriteColorPair> _equipmentSprites;
                public Spanning<BoneSpriteColorPair> EquipmentSprites => _equipmentSprites;
            }

            [System.Serializable]
            private class BoneSpriteColorPair
            {
                [SerializeField] private BoneSprite _sprite;
                public BoneSprite Sprite => _sprite;

                [SerializeField] private Color _color;
                public Color Color => _color;
            }
        }
    }
}
