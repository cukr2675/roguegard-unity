using HSVPicker;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lysionium.Views
{
    [AddComponentMenu("UI/Lysionium/Subviews/LUI Color Picker Subview")]
    public class ColorPickerSubview : Subview, IColorPickerSubview
    {
        [SerializeField] private ColorPicker _colorPicker = null;
        public ColorPicker ColorPicker => _colorPicker;
        [SerializeField] private Button _closeButton = null;
        [SerializeField] private Selectable _initialSelectable = null;

        private IColorPickerSubview.ColorPickerEventHandler onClose;
        private StateProvider currentStateProvider;

        protected override void CommonInitCore()
        {
            _closeButton.onClick.AddListener(() => onClose?.Invoke(Manager, Arg, _colorPicker.CurrentColor));
        }

        public void SetupColorPicker(
            Color color, IColorPickerSubview.ColorPickerEventHandler onClose, IListuiManager manager, IListuiArg arg,
            ref ISubviewStateProvider stateProvider)
        {
            stateProvider ??= new StateProvider();
            if (stateProvider is not StateProvider local) throw new System.ArgumentException(
                $"{stateProvider} は {nameof(StateProvider)} ではありません。");

            // 現在の StateProvider を外す前に状態を保存する
            if (currentStateProvider != null)
            {
            }

            // 表示更新
            _colorPicker.CurrentColor = color;
            this.onClose = onClose;
            SetArg(manager, arg);

            // 新しい StateProvider に切り替える
            currentStateProvider = local;
            EventSystem.SetSelectedGameObject(_initialSelectable.gameObject);
        }

        private class StateProvider : ISubviewStateProvider
        {
            public void Reset()
            {
            }
        }
    }
}
