using UnityEngine;

namespace OchalikeSprites
{
    [CreateAssetMenu(menuName = "Ochalike Sprites/Ochalike Morph")]
    public class OchalikeMorphAsset : ScriptableObject
    {
        [SerializeField] private Item[] _items = null;

#if UNITY_EDITOR
        [Header("Editor Only")]
        [SerializeField] internal OchalikeSpriteAsset _previewOchalikeSprite = null; // PropertyDrawer で使用
#endif

        public void AddTo(OchalikeMorph ochalikeMorph)
        {
            foreach (var item in _items)
            {
                // BareSprite/Color が設定されている場合のみ EquipmentList.Clear して設定
                if (item.MorphBareSprite != null || item.MorphBareColor != null)
                {
                    ochalikeMorph.SetBareSprite(item.Name, item.MorphBareSprite, item.MorphBareColor, item.OverridesOnDefaultColor);
                }

                for (int i = 0; i < item.EquipmentSprites.Length; i++)
                {
                    var equipmentPair = item.EquipmentSprites[i];
                    ochalikeMorph.AddEquipmentSprite(item.Name, equipmentPair.Sprite, equipmentPair.Color, item.OverridesOnDefaultColor);
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
                    ochalikeMorph.SetBareSprite(item.Name, item.MorphBareSprite, item.GetMorphBareColor(toColor), item.OverridesOnDefaultColor);
                }

                for (int i = 0; i < item.EquipmentSprites.Length; i++)
                {
                    var equipmentPair = item.EquipmentSprites[i];
                    ochalikeMorph.AddEquipmentSprite(item.Name, equipmentPair.Sprite, equipmentPair.GetColor(toColor), item.OverridesOnDefaultColor);
                }
            }
        }

        [System.Serializable]
        private class Item
        {
            [SerializeField] private BoneKeywordAsset _name;
            internal BoneKeyword Name => _name;

            [Tooltip("BareSprite を上書きする")]
            [SerializeField] private bool _hasMorphBareSprite;
            [SerializeField, VisibleBy(nameof(_hasMorphBareSprite))] private BoneSprite _morphBareSprite;
            internal BoneSprite MorphBareSprite => _hasMorphBareSprite ? _morphBareSprite : null;

            [Tooltip("BareColor を上書きする")]
            [SerializeField] private bool _hasMorphBareColor;
            [SerializeField, VisibleBy(nameof(_hasMorphBareColor), nameof(_morphBareColorIsFixed))] private Color _morphBareColor;
            internal Color? MorphBareColor => _hasMorphBareColor ? _morphBareColor : null;

            [Tooltip("この値が true のとき着色の対象外となる")]
            [SerializeField, HideInInspector] private bool _morphBareColorIsFixed;

            [SerializeField] private bool _overridesOnDefaultColor;
            internal bool OverridesOnDefaultColor => _overridesOnDefaultColor;

            [SerializeField] private BoneSpriteColorPair[] _equipmentSprites;
            internal System.ReadOnlySpan<BoneSpriteColorPair> EquipmentSprites => _equipmentSprites;

            internal Color? GetMorphBareColor(Color toColor)
            {
                if (!_hasMorphBareColor) return null;
                else if (_morphBareColorIsFixed) return _morphBareColor;
                else return toColor;
            }
        }

        [System.Serializable]
        private class BoneSpriteColorPair
        {
            [SerializeField] private BoneSprite _sprite;
            internal BoneSprite Sprite => _sprite;

            [SerializeField, VisibleBy(null, nameof(_colorIsFixed))] private Color _color;
            internal Color Color => _color;

            [SerializeField, HideInInspector] private bool _colorIsFixed;
            internal bool ColorIsFixed => _colorIsFixed;

            internal Color GetColor(Color toColor)
            {
                if (_colorIsFixed) return _color;
                else return toColor;
            }
        }
    }
}
