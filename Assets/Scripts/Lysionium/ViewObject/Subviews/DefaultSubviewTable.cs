using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lysionium.Views
{
    [AddComponentMenu("UI/Lysionium/LUI Default Subview Table")]
    public class DefaultSubviewTable : MonoBehaviour
    {
        [SerializeField] private Image _blocker = null;

        [SerializeField] private Subview _indicator = null;
        public Subview Indicator
        {
            get => _indicator;
            set => _indicator = value;
        }

        [SerializeField] private Subview _scroll = null;
        public Subview Scroll
        {
            get => _scroll;
            set => _scroll = value;
        }

        [SerializeField] private Subview _widgets = null;
        public Subview Widgets
        {
            get => _widgets;
            set => _widgets = value;
        }

        [SerializeField] private LongMessageSubview _longMessage = null;
        public LongMessageSubview LongMessage
        {
            get => _longMessage;
            set => _longMessage = value;
        }

        [SerializeField] private Subview _backAnchor = null;
        public Subview BackAnchor
        {
            get => _backAnchor;
            set => _backAnchor = value;
        }

        [SerializeField] private Subview _forwardAnchor = null;
        public Subview ForwardAnchor
        {
            get => _forwardAnchor;
            set => _forwardAnchor = value;
        }

        [SerializeField] private Subview _primaryCommand = null;
        public Subview PrimaryCommand
        {
            get => _primaryCommand;
            set => _primaryCommand = value;
        }

        [SerializeField] private Subview _captionBox = null;
        public Subview CaptionBox
        {
            get => _captionBox;
            set => _captionBox = value;
        }

        [SerializeField] private Subview _secondaryCommand = null;
        public Subview SecondaryCommand
        {
            get => _secondaryCommand;
            set => _secondaryCommand = value;
        }

        [SerializeField] private Subview _dialog = null;
        public Subview Dialog
        {
            get => _dialog;
            set => _dialog = value;
        }

        [SerializeField] private ColorPickerSubview _colorPicker = null;
        public ColorPickerSubview ColorPicker
        {
            get => _colorPicker;
            set => _colorPicker = value;
        }

        [SerializeField] private MessageBoxSubview _messageBox = null; // Rgpack の rg.msg で表示するため名前はメッセージボックス
        public MessageBoxSubview MessageBox
        {
            get => _messageBox;
            set => _messageBox = value;
        }

        [SerializeField] private Subview _fadeMask = null;
        public Subview FadeMask
        {
            get => _fadeMask;
            set => _fadeMask = value;
        }

        [SerializeField] private Subview _overlay = null;
        public Subview Overlay
        {
            get => _overlay;
            set => _overlay = value;
        }

        [SerializeField] private MessageBoxSubview _speechBox = null; // Rgpack の rg.say で表示するため名前はスピーチボックス
        public MessageBoxSubview SpeechBox
        {
            get => _speechBox;
            set => _speechBox = value;
        }

        [SerializeField] private Subview _choices = null;
        public Subview Choices
        {
            get => _choices;
            set => _choices = value;
        }

        [SerializeField] private DropdownScrollSubview _dropdownList = null;
        public DropdownScrollSubview DropdownList
        {
            get => _dropdownList;
            set => _dropdownList = value;
        }

        [SerializeField] private DropdownGridSubview _dropdownGrid = null;
        public DropdownGridSubview DropdownGrid
        {
            get => _dropdownGrid;
            set => _dropdownGrid = value;
        }

        public IReadOnlyList<Subview> Subviews { get; private set; }

        /// <summary>
        /// いずれかの <see cref="Subview.HasManagerLock"/> が true のとき true を取得する
        /// </summary>
        public bool HasManagerLock
        {
            get
            {
                for (int i = 0; i < Subviews.Count; i++)
                {
                    if (Subviews[i].HasManagerLock) return true;
                }
                return false;
            }
        }

        private bool isInitialized;

        public void CommonInit()
        {
            LuiAssert.NotInitialized(this, isInitialized);
            isInitialized = true;

            _indicator.CommonInit();
            _scroll.CommonInit();
            _widgets.CommonInit();
            _longMessage.CommonInit();
            _backAnchor.CommonInit();
            _forwardAnchor.CommonInit();
            _primaryCommand.CommonInit();
            _captionBox.CommonInit();
            _secondaryCommand.CommonInit();
            _dialog.CommonInit();
            _colorPicker.CommonInit();
            _messageBox.CommonInit();
            _fadeMask.CommonInit();
            _overlay.CommonInit();
            _speechBox.CommonInit();
            _choices.CommonInit();
            _dropdownList.CommonInit();
            _dropdownGrid.CommonInit();

            Subviews = new Subview[]
            {
                _indicator,
                _scroll,
                _widgets,
                _longMessage,
                _backAnchor,
                _forwardAnchor,
                _primaryCommand,
                _captionBox,
                _secondaryCommand,
                _dialog,
                _colorPicker,
                _messageBox,
                _fadeMask,
                _overlay,
                _speechBox,
                _choices,
                _dropdownList,
                _dropdownGrid,
            };
        }

        public void SetInvisibleDropdownPosition(Rect rect)
        {
            if (!_dropdownList.IsVisible) { _dropdownList.SetPosition(rect); }
            if (!_dropdownGrid.IsVisible) { _dropdownGrid.SetPosition(rect); }
        }

        public void SetBlocker(bool block)
        {
            _blocker.raycastTarget = block;
        }
    }
}
