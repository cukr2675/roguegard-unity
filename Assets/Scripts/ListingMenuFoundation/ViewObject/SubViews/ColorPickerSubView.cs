using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using HSVPicker;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/Sub Views/LMF Color Picker Sub View")]
    public class ColorPickerSubView : ElementsSubView
    {
        [SerializeField] private ColorPicker _colorPicker = null;
        public ColorPicker ColorPicker => _colorPicker;
        [SerializeField] private Button _closeButton = null;
        private bool isInitialized;

        private HandleClose handleClose;
        private StateProvider currentStateProvider;

        public delegate void HandleClose(IListMenuManager manager, IListMenuArg arg, Color color);

        public void Initialize()
        {
            LMFAssert.NotInitialized(this, isInitialized);
            isInitialized = true;

            _closeButton.onClick.AddListener(() => handleClose?.Invoke(Manager, Arg, _colorPicker.CurrentColor));
        }

        public override void SetParameters(
            IReadOnlyList<object> list, IElementHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref IElementsSubViewStateProvider stateProvider)
            => throw new System.NotSupportedException();

        public void SetParameters(
            Color color, HandleClose onClose, IListMenuManager manager, IListMenuArg arg,
            ref IElementsSubViewStateProvider stateProvider)
        {
            if (stateProvider == null) { stateProvider = new StateProvider(); }
            if (!(stateProvider is StateProvider local)) throw new System.ArgumentException(
                $"{stateProvider} �� {nameof(StateProvider)} �ł͂���܂���B");

            // ���݂� StateProvider ���O���O�ɏ�Ԃ�ۑ�����
            if (currentStateProvider != null)
            {
            }

            // �\���X�V
            _colorPicker.CurrentColor = color;
            this.handleClose = onClose;
            SetArg(manager, arg);
            SetStatusCode(0);

            // �V���� StateProvider �ɐ؂�ւ���
            currentStateProvider = local;
        }

        private class StateProvider : IElementsSubViewStateProvider
        {
            public void Reset()
            {
            }
        }
    }
}
