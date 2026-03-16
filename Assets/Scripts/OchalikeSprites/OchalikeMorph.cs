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
        private readonly Dictionary<BoneKeyword, ItemCore> items = new();

        private static readonly Stack<ItemCore> itemPool = new();

        private static readonly ItemCore emptyItem = new();

        public bool Any => items.Count >= 1;

        public Item GetSprite(BoneKeyword name)
        {
            if (items.TryGetValue(name, out var item))
            {
                return new Item(item);
            }
            else
            {
                return new Item(emptyItem);
            }
        }

        private ItemCore CreateItem()
        {
            if (!itemPool.TryPop(out var item))
            {
                item = new ItemCore();
            }
            return item;
        }

        /// <summary>
        /// <see cref="ItemCore.wearSprites"/> をクリアして <see cref="ItemCore.MorphBareSprite"/> を設定する
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
            item.wearSprites.Clear();
            item.wearColors.Clear();
        }

        /// <param name="overridesOnDefaultColor">
        /// true かつ <paramref name="color"/> の不透明度が 100% のとき素体のスプライトの色をベースカラーから上書きする。
        /// </param>
        public void AddWearSprite(BoneKeyword name, BoneSprite sprite, Color color, bool overridesOnDefaultColor = false)
        {
            if (sprite == null) throw new System.ArgumentNullException(nameof(sprite));

            if (!items.TryGetValue(name, out var item))
            {
                item = CreateItem();
                items.Add(name, item);
                item.wearSprites.Clear();
                item.wearColors.Clear();
            }
            item.OverridesOnDefaultColor |= overridesOnDefaultColor && color.a >= 1f;
            item.wearSprites.Add(sprite);
            item.wearColors.Add(color);
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
                    item.wearSprites.Clear();
                    item.wearColors.Clear();
                    ochalikeMorph.items.Add(pair.Key, item);
                }
                var value = pair.Value;
                item.MorphBareSprite = value.MorphBareSprite ?? item.MorphBareSprite;
                item.MorphBareColor = value.MorphBareColor ?? item.MorphBareColor;
                if (value.MorphBareSprite != null || value.MorphBareColor != null)
                {
                    item.OverridesOnDefaultColor = value.OverridesOnDefaultColor;
                    item.wearSprites.Clear();
                    item.wearColors.Clear();
                }
                foreach (var wearSprite in value.wearSprites)
                {
                    item.wearSprites.Add(wearSprite);
                }
                foreach (var equipmentColor in value.wearColors)
                {
                    item.wearColors.Add(equipmentColor);

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
                    item.wearSprites.Clear();
                    item.wearColors.Clear();
                    ochalikeMorph.items.Add(pair.Key, item);
                }
                var value = pair.Value;
                item.MorphBareSprite = value.MorphBareSprite ?? item.MorphBareSprite;
                item.MorphBareColor = value.MorphBareColor != null ? toColor : item.MorphBareColor; // 色を上書きするかは元テーブルによる
                if (value.MorphBareSprite != null || value.MorphBareColor != null)
                {
                    item.OverridesOnDefaultColor = value.OverridesOnDefaultColor;
                    item.wearSprites.Clear();
                    item.wearColors.Clear();
                }
                foreach (var wearSprite in value.wearSprites)
                {
                    item.wearSprites.Add(wearSprite);
                    item.wearColors.Add(toColor);
                }

                // 不透明のスプライトを重ねるときのみベースカラーの設定を上書きする
                if (value.wearSprites.Count >= 1 && toColor.a >= 1f) { item.OverridesOnDefaultColor |= value.OverridesOnDefaultColor; }
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
        /// <see cref="Item"/> のコンストラクタを internal で <see cref="OchalikeMorph"/> に公開する必要があるため、
        /// このクラスも internal にする
        /// <see cref="OchalikeMorph"/> 以外では使用しない
        /// </summary>
        internal sealed class ItemCore
        {
            /// <summary>
            /// 命名メモ: <see cref="SpritePoseBoneTransform.PoseBareSprite"/> と区別するため Morph をつける
            /// </summary>
            public BoneSprite MorphBareSprite { get; set; }
            public Color? MorphBareColor { get; set; }
            public bool OverridesOnDefaultColor { get; set; }
            public readonly List<BoneSprite> wearSprites = new(); // 命名メモ: 装備品でも BaseSprite を使用することがあるため EquipmentSprites は不適切
            public readonly List<Color> wearColors = new();

            public int WearSpriteCount => wearSprites.Count;

            public void GetWearSprite(int index, out BoneSprite sprite, out Color color)
            {
                sprite = wearSprites[index];
                color = wearColors[index];
            }
        }

        public readonly ref struct Item
        {
            private readonly ItemCore item;

            public BoneSprite MorphBareSprite => item.MorphBareSprite;
            public Color? MorphBareColor => item.MorphBareColor;
            public bool OverridesOnDefaultColor => item.OverridesOnDefaultColor;
            public int WearSpriteCount => item.WearSpriteCount;

            internal Item(ItemCore item) => this.item = item;

            public void GetWearSprite(int index, out BoneSprite sprite, out Color color) => item.GetWearSprite(index, out sprite, out color);
        }
    }
}
