using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    [CreateAssetMenu(menuName = "Ochalike Sprites/Ochalike Morph")]
    public class OchalikeMorphData : ScriptableObject
    {
        [SerializeField] private Item[] _items = null;

        public void AddTo(OchalikeMorph ochalikeMorph)
        {
            foreach (var item in _items)
            {
                // BareSprite/Color が設定されている場合のみ EquipmentList.Clear して設定
                if (item.MorphBareSprite != null || item.MorphBareColor != null)
                {
                    ochalikeMorph.SetFirstSprite(item.Name, item.MorphBareSprite, item.MorphBareColor, item.OverridesBaseColor);
                }

                for (int i = 0; i < item.EquipmentSprites.Length; i++)
                {
                    var equipmentPair = item.EquipmentSprites[i];
                    ochalikeMorph.AddEquipmentSprite(item.Name, equipmentPair.Sprite, equipmentPair.Color, item.OverridesBaseColor);
                }
            }
        }

        public void ColoredAddTo(OchalikeMorph ochalikeMorph, Color toColor)
        {
            foreach (var item in _items)
            {
                // BareSprite/Color が設定されている場合のみ EquipmentList.Clear して設定
                if (item.MorphBareSprite != null || item.MorphBareColor != null)
                {
                    ochalikeMorph.SetFirstSprite(item.Name, item.MorphBareSprite, item.GetMorphBareColor(toColor), item.OverridesBaseColor);
                }

                for (int i = 0; i < item.EquipmentSprites.Length; i++)
                {
                    var equipmentPair = item.EquipmentSprites[i];
                    ochalikeMorph.AddEquipmentSprite(item.Name, equipmentPair.Sprite, equipmentPair.GetColor(toColor), item.OverridesBaseColor);
                }
            }
        }

        [System.Serializable]
        public sealed class Item // エディタ拡張のために型だけ public にする
        {
            [SerializeField] private BoneKeywordData _name;
            internal BoneKeyword Name => _name;

            [Tooltip("BareSprite を上書きする")]
            [SerializeField] private bool _hasFirstSprite;
            internal bool HasFirstSprite => _hasFirstSprite; // TODO: 

            [SerializeField] private BoneSprite _firstSprite;
            internal BoneSprite FirstSprite => _firstSprite;

            //[Tooltip("BareColor を上書きする")]
            //[SerializeField] private bool _hasBareColor;
            //internal bool HasBareColor => _hasBareColor;

            [SerializeField] private Color _firstColor;
            internal Color FirstColor => _firstColor;

            internal BoneSprite MorphBareSprite => _hasFirstSprite ? _firstSprite : null;
            internal Color? MorphBareColor => _overridesSourceColor ? _firstColor : null;

            [Tooltip("この値が true のとき着色の対象外となる")]
            [SerializeField] private bool _firstColorIsFixed;
            internal bool FirstColorIsFixed => _firstColorIsFixed;

            [SerializeField] private bool _overridesSourceColor;
            //internal bool OverridesSourceColor => _overridesSourceColor;

            [SerializeField] private bool _overridesBaseColor;
            internal bool OverridesBaseColor => _overridesBaseColor;

            [SerializeField] private BoneSpriteColorPair[] _equipmentSprites;
            internal System.ReadOnlySpan<BoneSpriteColorPair> EquipmentSprites => _equipmentSprites;

            internal Color? GetMorphBareColor(Color toColor)
            {
                if (!_overridesSourceColor) return null;
                else if (_firstColorIsFixed) return _firstColor;
                else return toColor;
            }
        }

        [System.Serializable]
        public sealed class BoneSpriteColorPair // エディタ拡張のために型だけ public にする
        {
            [SerializeField] private BoneSprite _sprite;
            internal BoneSprite Sprite => _sprite;

            [SerializeField] private Color _color;
            internal Color Color => _color;

            [SerializeField] private bool _colorIsFixed;
            internal bool ColorIsFixed => _colorIsFixed;

            internal Color GetColor(Color toColor)
            {
                if (_colorIsFixed) return _color;
                else return toColor;
            }
        }
    }
}
