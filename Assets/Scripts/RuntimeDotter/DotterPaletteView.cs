using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UnityEngine.UI;

namespace RuntimeDotter
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class DotterPaletteView : MonoBehaviour
    {
        [SerializeField] private Button _itemPrefab = null;
        [SerializeField] private RectTransform _cursor = null;
        [SerializeField] private DotterColorPicker _picker = null;
        [SerializeField] private Button _mainColorButton = null;

        public DotterColorPicker Picker => _picker;

        private GridLayoutGroup layoutGroup;
        private Vector2 baseCellSize;
        private readonly List<Button> items = new List<Button>();

        private ShiftableColor _mainColor;
        public Color32 MainColor => _mainColor.ToPickerColor();
        private ShiftableColor[] _palette;
        public IReadOnlyList<ShiftableColor> Palette => _palette;

        public int ColorIndex { get; private set; }

        private bool editMainColor;

        private void Awake()
        {
            layoutGroup = GetComponent<GridLayoutGroup>();
            baseCellSize = layoutGroup.cellSize;

            _picker.OnColorChanged.AddListener(x => UpdateColor(x));
            _mainColorButton.onClick.AddListener(() => StartEditMainColor());
        }

        private void StartEditMainColor()
        {
            editMainColor = true;
            _picker.Open(_mainColor, true);
        }

        private void UpdateColor(ShiftableColor newColor)
        {
            if (editMainColor)
            {
                _mainColor = newColor;
            }
            else
            {
                _palette[ColorIndex] = newColor;
            }

            var color = _mainColor.ToPickerColor();
            var colors = _itemPrefab.colors;
            colors.normalColor *= color;
            colors.highlightedColor *= color;
            colors.pressedColor *= color;
            colors.selectedColor *= color;
            colors.disabledColor *= color;
            for (int i = 0; i < _palette.Length; i++)
            {
                var item = items[i];
                var sprite = _palette[i].ToIcon();
                item.image.sprite = sprite;
                item.colors = colors;
            }
            _mainColorButton.colors = colors;
        }

        public void Load(IReadOnlyList<ShiftableColor> palette, Color32 mainColor)
        {
            if (palette.Count == 0) throw new System.ArgumentException();

            _mainColor = new ShiftableColor(mainColor, false);
            _palette = palette.ToArray();

            while (items.Count < palette.Count)
            {
                var itemIndex = items.Count;
                var item = Instantiate(_itemPrefab, transform);
                item.onClick.AddListener(() => ClickItem(itemIndex));
                items.Add(item);
            }
            while (items.Count > palette.Count)
            {
                Destroy(items[items.Count - 1].gameObject);
            }

            var ratio = baseCellSize.x * 2f / ((RectTransform)transform).rect.width;
            layoutGroup.cellSize = baseCellSize * ratio;
            layoutGroup.CalculateLayoutInputHorizontal();
            layoutGroup.CalculateLayoutInputVertical();
            layoutGroup.SetLayoutHorizontal();
            layoutGroup.SetLayoutVertical();

            ClickItem(0);
            _picker.Close();
            UpdateColor(_palette[0]);
        }

        private void ClickItem(int index)
        {
            editMainColor = false;
            if (ColorIndex != index)
            {
                // 色選択
                ColorIndex = index;

                var item = (RectTransform)items[index].transform;
                _cursor.position = item.position;
                //_cursor.sizeDelta = item.sizeDelta;

                if (_picker.IsShow) { _picker.Open(_palette[index], false); }
            }
            else
            {
                // 同じ色をクリックしたら色編集を開く
                _picker.Open(_palette[index], false);
            }
        }
    }
}
