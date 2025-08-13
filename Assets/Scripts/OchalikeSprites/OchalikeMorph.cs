using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    // 用途: つけっぱなしにするもの
    // 例: キャラクリ、装備、持続エフェクト（混乱中のぐるぐる目など）

    // 命名メモ:
    // OchalikeWear だと BareSprite とかあるのが変なので OchalikeMorph
    // OchalikeMakeup だと服にも使うことがわかりにくくなる

    /// <summary>
    /// <see cref="OchalikeSpriteAsset"/> のボーン構造と位置はそのままに見た目を変更するクラス。
    /// 処理順は <see cref="OchalikeBone"/> と <see cref="SpritePose"/> の中間に位置する
    /// </summary>
    public class OchalikeMorph
    {
        private readonly Dictionary<BoneKeyword, Item> items = new();

        private static readonly Stack<Item> itemPool = new();

        private static readonly Item emptyItem = new();

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

        /// <summary>
        /// <see cref="Item.equipmentSprites"/> をクリアして <see cref="Item.MorphBareSprite"/> を設定する
        /// </summary>
        /// <param name="overridesOnDefaultColor">true のとき素体のスプライトの色をベースカラーから上書きする。</param>
        public void SetBareSprite(BoneKeyword name, BoneSprite morphBareSprite = null, Color? morphBareColor = null, bool overridesOnDefaultColor = false)
        {
            // AddTo の動作と合わせるため両方 null は例外を投げる
            if (morphBareSprite == null && morphBareColor == null) throw new System.ArgumentException(
                $"{nameof(morphBareSprite)} と {nameof(morphBareColor)} の両方を null にすることはできません。");

            if (!items.TryGetValue(name, out var item))
            {
                item = CreateItem();
                items.Add(name, item);
            }
            item.MorphBareSprite = morphBareSprite ?? item.MorphBareSprite;
            item.MorphBareColor = morphBareColor ?? item.MorphBareColor;
            item.OverridesOnDefaultColor = overridesOnDefaultColor;
            item.equipmentSprites.Clear();
            item.equipmentColors.Clear();
        }

        /// <param name="overridesOnDefaultColor">true かつ <paramref name="color"/> の不透明度が 100% のとき素体のスプライトの色をベースカラーから上書きする。</param>
        public void AddEquipmentSprite(BoneKeyword name, BoneSprite sprite, Color color, bool overridesOnDefaultColor = false)
        {
            if (sprite == null) throw new System.ArgumentNullException(nameof(sprite));

            if (!items.TryGetValue(name, out var item))
            {
                item = CreateItem();
                items.Add(name, item);
                item.equipmentSprites.Clear();
                item.equipmentColors.Clear();
            }
            item.OverridesOnDefaultColor |= overridesOnDefaultColor && color.a >= 1f;
            item.equipmentSprites.Add(sprite);
            item.equipmentColors.Add(color);
        }

        public void AddTo(OchalikeMorph ochalikeMorph)
        {
            foreach (var pair in items)
            {
                if (!ochalikeMorph.items.TryGetValue(pair.Key, out var item))
                {
                    // 同じキーの項目が存在しない場合は追加する。
                    item = CreateItem();
                    item.MorphBareSprite = null;
                    item.MorphBareColor = null;
                    item.OverridesOnDefaultColor = false;
                    item.equipmentSprites.Clear();
                    item.equipmentColors.Clear();
                    ochalikeMorph.items.Add(pair.Key, item);
                }
                var value = pair.Value;
                item.MorphBareSprite = value.MorphBareSprite ?? item.MorphBareSprite;
                item.MorphBareColor = value.MorphBareColor ?? item.MorphBareColor;
                if (value.MorphBareSprite != null || value.MorphBareColor != null)
                {
                    item.OverridesOnDefaultColor = value.OverridesOnDefaultColor;
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
                    if (equipmentColor.a >= 1f) { item.OverridesOnDefaultColor |= value.OverridesOnDefaultColor; }
                }
            }
        }

        public void ColoredAddTo(OchalikeMorph ochalikeMorph, Color toColor)
        {
            foreach (var pair in items)
            {
                if (!ochalikeMorph.items.TryGetValue(pair.Key, out var item))
                {
                    // 同じキーの項目が存在しない場合は追加する。
                    item = CreateItem();
                    item.MorphBareSprite = null;
                    item.MorphBareColor = null;
                    item.OverridesOnDefaultColor = false;
                    item.equipmentSprites.Clear();
                    item.equipmentColors.Clear();
                    ochalikeMorph.items.Add(pair.Key, item);
                }
                var value = pair.Value;
                item.MorphBareSprite = value.MorphBareSprite ?? item.MorphBareSprite;
                item.MorphBareColor = value.MorphBareColor != null ? toColor : item.MorphBareColor; // 色を上書きするかは元テーブルによる
                if (value.MorphBareSprite != null || value.MorphBareColor != null)
                {
                    item.OverridesOnDefaultColor = value.OverridesOnDefaultColor;
                    item.equipmentSprites.Clear();
                    item.equipmentColors.Clear();
                }
                foreach (var equipmentSprite in value.equipmentSprites)
                {
                    item.equipmentSprites.Add(equipmentSprite);
                    item.equipmentColors.Add(toColor);
                }

                // 不透明のスプライトを重ねるときのみベースカラーの設定を上書きする
                if (value.equipmentSprites.Count >= 1 && toColor.a >= 1f) { item.OverridesOnDefaultColor |= value.OverridesOnDefaultColor; }
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
        /// <see cref="RefItem"/> のコンストラクタを internal で <see cref="OchalikeMorph"/> に公開する必要があるため、
        /// このクラスも internal にする
        /// <see cref="OchalikeMorph"/> 以外では使用しない
        /// </summary>
        internal sealed class Item
        {
            public BoneSprite MorphBareSprite { get; set; }
            public Color? MorphBareColor { get; set; }
            public bool OverridesOnDefaultColor { get; set; }
            public readonly List<BoneSprite> equipmentSprites = new();
            public readonly List<Color> equipmentColors = new();

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

            public BoneSprite MorphBareSprite => item.MorphBareSprite;
            public Color? MorphBareColor => item.MorphBareColor;
            public bool OverridesOnDefaultColor => item.OverridesOnDefaultColor;
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
