using HSVPicker;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lysionium
{
    [AddComponentMenu("UI/Lysionium/Subviews/LUI Color Picker Subview")]
    public class ColorPickerSubview : Subview
    {
        [SerializeField] private ColorPicker _colorPicker = null;
        public ColorPicker ColorPicker => _colorPicker;
        [SerializeField] private Button _closeButton = null;
        [SerializeField] private Selectable _initialSelectable = null;

        private ColorPickerEventHandler onClose;
        private StateProvider currentStateProvider;

        public delegate void ColorPickerEventHandler(IListMenuManager manager, IListMenuArg arg, Color color);

        protected override void CommonInitCore()
        {
            _closeButton.onClick.AddListener(() => onClose?.Invoke(Manager, Arg, _colorPicker.CurrentColor));
        }

        public override void SetParameters(
            IReadOnlyList<object> list, IViewItemHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref ISubviewStateProvider stateProvider)
            => throw new System.NotSupportedException();

        public void SetParameters(
            Color color, ColorPickerEventHandler onClose, IListMenuManager manager, IListMenuArg arg,
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
            SetStatusCode(0);

            // 新しい StateProvider に切り替える
            currentStateProvider = local;
            EventSystem.current.SetSelectedGameObject(_initialSelectable.gameObject);
        }

        private class StateProvider : ISubviewStateProvider
        {
            public void Reset()
            {
            }
        }
    }
}
