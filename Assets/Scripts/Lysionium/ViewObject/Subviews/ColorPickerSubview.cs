using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using HSVPicker;

namespace Lysionium
{
    [AddComponentMenu("UI/Lysionium/Subviews/LUI Color Picker Subview")]
    public class ColorPickerSubview : ElementsSubview
    {
        [SerializeField] private ColorPicker _colorPicker = null;
        public ColorPicker ColorPicker => _colorPicker;
        [SerializeField] private Button _closeButton = null;
        [SerializeField] private Selectable _initialSelectable = null;

        private HandleClose handleClose;
        private StateProvider currentStateProvider;

        public delegate void HandleClose(IListMenuManager manager, IListMenuArg arg, Color color);

        protected override void CommonInitCore()
        {
            _closeButton.onClick.AddListener(() => handleClose?.Invoke(Manager, Arg, _colorPicker.CurrentColor));
        }

        public override void SetParameters(
            IReadOnlyList<object> list, IElementHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref IElementsSubviewStateProvider stateProvider)
            => throw new System.NotSupportedException();

        public void SetParameters(
            Color color, HandleClose onClose, IListMenuManager manager, IListMenuArg arg,
            ref IElementsSubviewStateProvider stateProvider)
        {
            if (stateProvider == null) { stateProvider = new StateProvider(); }
            if (!(stateProvider is StateProvider local)) throw new System.ArgumentException(
                $"{stateProvider} は {nameof(StateProvider)} ではありません。");

            // 現在の StateProvider を外す前に状態を保存する
            if (currentStateProvider != null)
            {
            }

            // 表示更新
            _colorPicker.CurrentColor = color;
            handleClose = onClose;
            SetArg(manager, arg);
            SetStatusCode(0);

            // 新しい StateProvider に切り替える
            currentStateProvider = local;
            EventSystem.current.SetSelectedGameObject(_initialSelectable.gameObject);
        }

        private class StateProvider : IElementsSubviewStateProvider
        {
            public void Reset()
            {
            }
        }
    }
}
