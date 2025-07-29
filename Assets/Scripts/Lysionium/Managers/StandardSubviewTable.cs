using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace Lysionium
{
    [AddComponentMenu("UI/Lysionium/LUI Standard Subview Table")]
    public class StandardSubviewTable : MonoBehaviour
    {
        [SerializeField] private Image _blocker = null;

        [SerializeField] private Subview _scroll = null;
        public ISubview Scroll => _scroll;
        public static string ScrollName => "Scroll";

        [SerializeField] private Subview _widgets = null;
        public ISubview Widgets => _widgets;
        public static string WidgetsName => "Widgets";

        [SerializeField] private LongMessageSubview _longMessage = null;
        public MessageBoxSubview LongMessage => _longMessage;
        public static string LongMessageName => "LongMessage";

        [SerializeField] private Subview _backAnchor = null;
        public ISubview BackAnchor => _backAnchor;
        public static string BackAnchorName => "BackAnchor";

        [SerializeField] private Subview _forwardAnchor = null;
        public ISubview ForwardAnchor => _forwardAnchor;
        public static string ForwardAnchorName => "ForwardAnchor";

        [SerializeField] private Subview _primaryCommand = null;
        public ISubview PrimaryCommand => _primaryCommand;
        public static string PrimaryCommandName => "PrimaryCommand";

        [SerializeField] private Subview _captionBox = null;
        public ISubview CaptionBox => _captionBox;
        public static string CaptionBoxName => "CaptionBox";

        [SerializeField] private Subview _secondaryCommand = null;
        public ISubview SecondaryCommand => _secondaryCommand;
        public static string SecondaryCommandName => "SecondaryCommand";

        [SerializeField] private Subview _dialog = null;
        public ISubview Dialog => _dialog;
        public static string DialogName => "Dialog";

        [SerializeField] private Subview _colorPicker = null;
        public ISubview ColorPicker => _colorPicker;
        public static string ColorPickerName => "ColorPicker";

        [SerializeField] private MessageBoxSubview _messageBox = null; // Rgpack の rg.msg で表示するため名前はメッセージボックス
        public MessageBoxSubview MessageBox => _messageBox;
        public static string MessageBoxName => "MessageBox";

        [SerializeField] private Subview _fadeMask = null;
        public ISubview FadeMask => _fadeMask;
        public static string FadeMaskName => "FadeMask";

        [SerializeField] private Subview _overlay = null;
        public ISubview Overlay => _overlay;
        public static string OverlayName => "Overlay";

        [SerializeField] private MessageBoxSubview _speechBox = null; // Rgpack の rg.say で表示するため名前はスピーチボックス
        public MessageBoxSubview SpeechBox => _speechBox;
        public static string SpeechBoxName => "SpeechBox";

        [SerializeField] private Subview _choices = null;
        public ISubview Choices => _choices;
        public static string ChoicesName => "Choices";

        public IReadOnlyDictionary<string, Subview> Subviews { get; private set; }

        /// <summary>
        /// いずれかの <see cref="Subview.HasManagerLock"/> が true のとき true を取得する
        /// </summary>
        public bool HasManagerLock
        {
            get
            {
                foreach (var pair in Subviews)
                {
                    if (pair.Value.HasManagerLock) return true;
                }
                return false;
            }
        }

        private bool isInitialized;

        public void CommonInit()
        {
            LuiAssert.NotInitialized(this, isInitialized);
            isInitialized = true;

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

            Subviews = new Dictionary<string, Subview>()
            {
                { ScrollName, _scroll },
                { WidgetsName, _widgets },
                { LongMessageName, _longMessage },
                { BackAnchorName, _backAnchor },
                { ForwardAnchorName, _forwardAnchor },
                { PrimaryCommandName, _primaryCommand },
                { CaptionBoxName, _captionBox },
                { SecondaryCommandName, _secondaryCommand },
                { DialogName, _dialog },
                { ColorPickerName, _colorPicker },
                { MessageBoxName, _messageBox },
                { SpeechBoxName, _speechBox },
                { ChoicesName, _choices },
                { FadeMaskName, _fadeMask },
                { OverlayName, _overlay },
            };
        }

        public void SetBlocker(bool block)
        {
            _blocker.raycastTarget = block;
        }
    }
}
