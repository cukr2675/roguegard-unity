using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lysionium.Views
{
    [AddComponentMenu("UI/Lysionium/LUI Default Subview Table")]
    public class DefaultSubviewTable : MonoBehaviour
    {
        [SerializeField] private Image _blocker = null;

        [Tooltip("ゲームプレイ中（メニュー非表示時）だけ表示する Subview 。メニュー表示中も表示させる HUD は Subview の外に追加する")]
        [SerializeField] private ListHandlerSubview _playingHud = null;
        public ListHandlerSubview PlayingHud
        {
            get => _playingHud;
            set => _playingHud = value;
        }

        [SerializeField] private ListHandlerSubview _scroll = null;
        public ListHandlerSubview Scroll
        {
            get => _scroll;
            set => _scroll = value;
        }

        [SerializeField] private ListHandlerSubview _widgets = null;
        public ListHandlerSubview Widgets
        {
            get => _widgets;
            set => _widgets = value;
        }

        [SerializeField] private MessageBoxSubview _longMessage = null;
        public MessageBoxSubview LongMessage
        {
            get => _longMessage;
            set => _longMessage = value;
        }

        [SerializeField] private ListHandlerSubview _backAnchor = null;
        public ListHandlerSubview BackAnchor
        {
            get => _backAnchor;
            set => _backAnchor = value;
        }

        [SerializeField] private ListHandlerSubview _forwardAnchor = null;
        public ListHandlerSubview ForwardAnchor
        {
            get => _forwardAnchor;
            set => _forwardAnchor = value;
        }

        [SerializeField] private ListHandlerSubview _primaryCommand = null;
        public ListHandlerSubview PrimaryCommand
        {
            get => _primaryCommand;
            set => _primaryCommand = value;
        }

        [SerializeField] private MessageBoxSubview _captionBox = null;
        public MessageBoxSubview CaptionBox
        {
            get => _captionBox;
            set => _captionBox = value;
        }

        [SerializeField] private ListHandlerSubview _secondaryCommand = null;
        public ListHandlerSubview SecondaryCommand
        {
            get => _secondaryCommand;
            set => _secondaryCommand = value;
        }

        [SerializeField] private ListHandlerSubview _dialog = null;
        public ListHandlerSubview Dialog
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

        [SerializeField] private ListHandlerSubview _fadeMask = null;
        public ListHandlerSubview FadeMask
        {
            get => _fadeMask;
            set => _fadeMask = value;
        }

        [SerializeField] private ListHandlerSubview _overlay = null;
        public ListHandlerSubview Overlay
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

        [SerializeField] private ListHandlerSubview _choices = null;
        public ListHandlerSubview Choices
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

            _playingHud.CommonInit();
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
                _playingHud,
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
