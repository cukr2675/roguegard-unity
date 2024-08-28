using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDSSprite
{
    public class EffectableBoneSpriteTable
    {
        private readonly Dictionary<BoneKeyword, Item> items = new Dictionary<BoneKeyword, Item>();

        private static readonly Stack<Item> itemPool = new Stack<Item>();

        private static readonly Item emptyItem = new Item();

        public bool Any => items.Count >= 1;

        public RefItem GetSprite(BoneKeyword name)
        {
            if (items.TryGetValue(name, out var item))
            {
                return new RefItem(item);
            }
            else
            {
                return new RefItem(emptyItem);
            }
        }

        private Item CreateItem()
        {
            if (!itemPool.TryPop(out var item))
            {
                item = new Item();
            }
            return item;
        }

        /// <param name="overridesBaseColor">true のとき素体のスプライトの色をベースカラーから上書きする。</param>
        public void SetFirstSprite(BoneKeyword name, BoneSprite sprite, Color color, bool overridesBaseColor = false)
        {
            if (sprite == null) throw new System.ArgumentNullException(nameof(sprite));

            if (!items.TryGetValue(name, out var item))
            {
                item = CreateItem();
                items.Add(name, item);
            }
            item.FirstSprite = sprite;
            item.FirstColor = color;
            item.OverridesSourceColor = true;
            item.OverridesBaseColor = overridesBaseColor;
            item.equipmentSprites.Clear();
            item.equipmentColors.Clear();
        }

        /// <summary>
        /// <paramref name="sprite"/> を素体のスプライトから上書きする。
        /// </summary>
        /// <param name="overridesBaseColor">true のとき素体のスプライトの色をベースカラーから上書きする。</param>
        public void SetFirstSprite(BoneKeyword name, BoneSprite sprite, bool overridesBaseColor = false)
        {
            if (sprite == null) throw new System.ArgumentNullException(nameof(sprite));

            if (!items.TryGetValue(name, out var item))
            {
                item = CreateItem();
                items.Add(name, item);
            }
            item.FirstSprite = sprite;
            item.FirstColor = default;
            item.OverridesSourceColor = false;
            item.OverridesBaseColor = overridesBaseColor;
            item.equipmentSprites.Clear();
            item.equipmentColors.Clear();
        }

        /// <summary>
        /// <paramref name="color"/> を素体のスプライトから上書きする。
        /// </summary>
        /// <param name="overridesBaseColor">true のとき素体のスプライトの色をベースカラーから上書きする。</param>
        public void SetFirstSprite(BoneKeyword name, Color color, bool overridesBaseColor = false)
        {
            if (!items.TryGetValue(name, out var item))
            {
                item = CreateItem();
                items.Add(name, item);
            }
            item.FirstSprite = null;
            item.FirstColor = color;
            item.OverridesSourceColor = true;
            item.OverridesBaseColor = overridesBaseColor;
            item.equipmentSprites.Clear();
            item.equipmentColors.Clear();
        }

        /// <param name="overridesBaseColor">true かつ <paramref name="color"/> の不透明度が 100% のとき素体のスプライトの色をベースカラーから上書きする。</param>
        public void AddEquipmentSprite(BoneKeyword name, BoneSprite sprite, Color color, bool overridesBaseColor = false)
        {
            if (sprite == null) throw new System.ArgumentNullException(nameof(sprite));

            if (!items.TryGetValue(name, out var item))
            {
                item = CreateItem();
                items.Add(name, item);
                item.equipmentSprites.Clear();
                item.equipmentColors.Clear();
            }
            item.OverridesBaseColor |= overridesBaseColor && color.a >= 1f;
            item.equipmentSprites.Add(sprite);
            item.equipmentColors.Add(color);
        }

        public void AddTo(EffectableBoneSpriteTable table)
        {
            foreach (var pair in items)
            {
                if (!table.items.TryGetValue(pair.Key, out var item))
                {
                    // 同じキーの項目が存在しない場合は追加する。
                    item = CreateItem();
                    item.FirstSprite = null;
                    item.FirstColor = default;
                    item.OverridesSourceColor = false;
                    item.OverridesBaseColor = false;
                    item.equipmentSprites.Clear();
                    item.equipmentColors.Clear();
                    table.items.Add(pair.Key, item);
                }
                var value = pair.Value;
                item.FirstSprite = value.FirstSprite ?? item.FirstSprite;
                item.FirstColor = value.OverridesSourceColor ? value.FirstColor : item.FirstColor;
                item.OverridesSourceColor |= value.OverridesSourceColor;
                if (value.FirstSprite != null || value.OverridesSourceColor)
                {
                    item.OverridesBaseColor = value.OverridesBaseColor;
                    item.equipmentSprites.Clear();
                    item.equipmentColors.Clear();
                }
                foreach (var equipmentSprite in value.equipmentSprites)
                {
                    item.equipmentSprites.Add(equipmentSprite);
                }
                foreach (var equipmentColor in value.equipmentColors)
                {
                    item.equipmentColors.Add(equipmentColor);

                    // 不透明のスプライトを重ねるときのみベースカラーの設定を上書きする
                    if (equipmentColor.a >= 1f) { item.OverridesBaseColor |= value.OverridesBaseColor; }
                }
            }
        }

        public void ColoredAddTo(EffectableBoneSpriteTable table, Color toColor)
        {
            foreach (var pair in items)
            {
                if (!table.items.TryGetValue(pair.Key, out var item))
                {
                    // 同じキーの項目が存在しない場合は追加する。
                    item = CreateItem();
                    item.FirstSprite = null;
                    item.FirstColor = default;
                    item.OverridesSourceColor = false;
                    item.OverridesBaseColor = false;
                    item.equipmentSprites.Clear();
                    item.equipmentColors.Clear();
                    table.items.Add(pair.Key, item);
                }
                var value = pair.Value;
                item.FirstSprite = value.FirstSprite ?? item.FirstSprite;
                item.FirstColor = value.OverridesSourceColor ? toColor : item.FirstColor;
                item.OverridesSourceColor |= value.OverridesSourceColor;
                if (value.FirstSprite != null || value.OverridesSourceColor)
                {
                    item.OverridesBaseColor = value.OverridesBaseColor;
                    item.equipmentSprites.Clear();
                    item.equipmentColors.Clear();
                }
                foreach (var equipmentSprite in value.equipmentSprites)
                {
                    item.equipmentSprites.Add(equipmentSprite);
                    item.equipmentColors.Add(toColor);
                }

                // 不透明のスプライトを重ねるときのみベースカラーの設定を上書きする
                if (value.equipmentSprites.Count >= 1 && toColor.a >= 1f) { item.OverridesBaseColor |= value.OverridesBaseColor; }
            }
        }

        public void Clear()
        {
            foreach (var pair in items)
            {
                itemPool.Push(pair.Value);
            }
            items.Clear();
        }

        /// <summary>
        /// <see cref="RefItem"/> のコンストラクタを internal で <see cref="EffectableBoneSpriteTable"/> に公開する必要があるため、
        /// このクラスも internal にする
        /// <see cref="EffectableBoneSpriteTable"/> 以外では使用しない
        /// </summary>
        internal sealed class Item
        {
            public BoneSprite FirstSprite { get; set; }
            public Color FirstColor { get; set; }
            public bool OverridesSourceColor { get; set; }
            public bool OverridesBaseColor { get; set; }
            public readonly List<BoneSprite> equipmentSprites = new List<BoneSprite>();
            public readonly List<Color> equipmentColors = new List<Color>();

            public int EquipmentSpriteCount => equipmentSprites.Count;

            public void GetEquipmentSprite(int index, out BoneSprite sprite, out Color color)
            {
                sprite = equipmentSprites[index];
                color = equipmentColors[index];
            }
        }

        public readonly ref struct RefItem
        {
            private readonly Item item;

            public BoneSprite FirstSprite => item.FirstSprite;
            public Color FirstColor => item.FirstColor;
            public bool OverridesSourceColor => item.OverridesSourceColor;
            public bool OverridesBaseColor => item.OverridesBaseColor;
            public int EquipmentSpriteCount => item.EquipmentSpriteCount;

            internal RefItem(Item item)
            {
                this.item = item;
            }

            public void GetEquipmentSprite(int index, out BoneSprite sprite, out Color color)
            {
                item.GetEquipmentSprite(index, out sprite, out color);
            }
        }
    }
}
