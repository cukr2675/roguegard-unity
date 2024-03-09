using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using UnityEngine.UI;
using HSVPicker;

namespace RuntimeDotter
{
    public class DotterColorPicker : MonoBehaviour
    {
        [SerializeField] private ColorPicker _normalPicker = null;
        [SerializeField] private ColorPicker _shiftPicker = null;
        [SerializeField] private CanvasGroup _normalGroup = null;
        [SerializeField] private CanvasGroup _shiftGroup = null;
        [SerializeField] private Button _closePickerButton0 = null;
        [SerializeField] private Button _closePickerButton1 = null;
        [SerializeField] private Button _switchPickerButton0 = null;
        [SerializeField] private Button _switchPickerButton1 = null;

        [SerializeField] private UnityEvent<ShiftableColor> _onColorChanged = null;
        public UnityEvent<ShiftableColor> OnColorChanged => _onColorChanged;

        public bool IsShow => _normalGroup.interactable || _shiftGroup.interactable;

        private ShiftableColor _currentColor;
        public ShiftableColor CurrentColor => _currentColor;

        private ColorPresetList normalPresetList;
        private ColorPresetList shiftPresetList;
        private bool afterOpen;
        private Color afterOpenColor;

        private void Awake()
        {
            // �v���Z�b�g����ɒB������ŌẪv���Z�b�g���폜����
            if (_normalPicker != null)
            {
                var normalPresets = _normalPicker.GetComponentInChildren<ColorPresets>();
                var normalPresetsLength = normalPresets.presets.Length;
                normalPresetList = ColorPresetManager.Get(_normalPicker.Setup.PresetColorsId);
                normalPresetList.OnColorsUpdated += x => { if (x.Count >= normalPresetsLength) { x.RemoveAt(0); } };
            }
            if (_shiftPicker != null)
            {
                var shiftPresets = _shiftPicker.GetComponentInChildren<ColorPresets>();
                var shiftPresetsLength = shiftPresets.presets.Length;
                shiftPresetList = ColorPresetManager.Get(_shiftPicker.Setup.PresetColorsId);
                shiftPresetList.OnColorsUpdated += x => { if (x.Count >= shiftPresetsLength) { x.RemoveAt(0); } };
            }

            _normalPicker?.onValueChanged.AddListener(x => UpdateColor(x));
            _shiftPicker?.onValueChanged.AddListener(x => UpdateColor(x));
            _closePickerButton0?.onClick.AddListener(() => Close());
            _closePickerButton1?.onClick.AddListener(() => Close());
            _switchPickerButton0?.onClick.AddListener(() => SwitchPicker());
            _switchPickerButton1?.onClick.AddListener(() => SwitchPicker());
        }

        private void UpdateColor(Color32 pickerColor)
        {
            if (afterOpen)
            {
                // �F�I���������ƍŏ��̐F�ҏW���ɂ��Ƃ̐F�𗚗��ɉ�����
                if (!_currentColor.IsShift && !normalPresetList.Colors.Contains(afterOpenColor)) { normalPresetList.AddColor(afterOpenColor); }
                if (_currentColor.IsShift && !shiftPresetList.Colors.Contains(afterOpenColor)) { shiftPresetList.AddColor(afterOpenColor); }
                afterOpen = false;
            }

            _currentColor = new ShiftableColor(pickerColor, _currentColor.IsShift);

            _onColorChanged.Invoke(_currentColor);
        }

        public void Close()
        {
            SetShow(_normalGroup, false);
            SetShow(_shiftGroup, false);
        }

        private void SwitchPicker()
        {
            afterOpen = false;
            _currentColor.IsShift = !_currentColor.IsShift;
            UpdateColor(_currentColor.ToPickerColor());
            UpdateActive();
        }

        public void Open(ShiftableColor color, bool pickMainColor)
        {
            _currentColor = color;

            afterOpen = false; // picker.CurrentColor �� setter �Ńv���Z�b�g���ǉ�����Ȃ��悤 false �ɂ���
            UpdateActive();
            afterOpen = true;
            afterOpenColor = color.ToPickerColor();

            _switchPickerButton0?.gameObject.SetActive(!pickMainColor);
            _switchPickerButton1?.gameObject.SetActive(!pickMainColor);
        }

        private void UpdateActive()
        {
            SetShow(_normalGroup, !_currentColor.IsShift);
            SetShow(_shiftGroup, _currentColor.IsShift);
            if (_currentColor.IsShift) { _shiftPicker.CurrentColor = _currentColor.ToPickerColor(); }
            else { _normalPicker.CurrentColor = _currentColor.ToPickerColor(); }
        }

        /// <summary>
        /// <see cref="GameObject.active"/> == false �ɂ���� <see cref="ColorPicker"/> �� Start �����s����Ȃ����� <see cref="CanvasGroup"/> ���g���B
        /// </summary>
        private static void SetShow(CanvasGroup group, bool show)
        {
            if (group == null) return;

            group.alpha = show ? 1f : 0f;
            group.interactable = show;
            group.blocksRaycasts = show;
        }
    }
}
