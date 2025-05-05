using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace Lysionium
{
    [AddComponentMenu("UI/Lysionium/LUI Standard Sub View Table")]
    public class StandardSubViewTable : MonoBehaviour
    {
        [SerializeField] private Image _blocker = null;

        [SerializeField] private ElementsSubView _scroll = null;
        public IElementsSubView Scroll => _scroll;
        public static string ScrollName => "Scroll";

        [SerializeField] private ElementsSubView _widgets = null;
        public IElementsSubView Widgets => _widgets;
        public static string WidgetsName => "Widgets";

        [SerializeField] private LongMessageSubView _longMessage = null;
        public MessageBoxSubView LongMessage => _longMessage;
        public static string LongMessageName => "LongMessage";

        [SerializeField] private ElementsSubView _backAnchor = null;
        public IElementsSubView BackAnchor => _backAnchor;
        public static string BackAnchorName => "BackAnchor";

        [SerializeField] private ElementsSubView _forwardAnchor = null;
        public IElementsSubView ForwardAnchor => _forwardAnchor;
        public static string ForwardAnchorName => "ForwardAnchor";

        [SerializeField] private ElementsSubView _primaryCommand = null;
        public IElementsSubView PrimaryCommand => _primaryCommand;
        public static string PrimaryCommandName => "PrimaryCommand";

        [SerializeField] private ElementsSubView _captionBox = null;
        public IElementsSubView CaptionBox => _captionBox;
        public static string CaptionBoxName => "CaptionBox";

        [SerializeField] private ElementsSubView _secondaryCommand = null;
        public IElementsSubView SecondaryCommand => _secondaryCommand;
        public static string SecondaryCommandName => "SecondaryCommand";

        [SerializeField] private ElementsSubView _dialog = null;
        public IElementsSubView Dialog => _dialog;
        public static string DialogName => "Dialog";

        [SerializeField] private ElementsSubView _colorPicker = null;
        public IElementsSubView ColorPicker => _colorPicker;
        public static string ColorPickerName => "ColorPicker";

        [SerializeField] private MessageBoxSubView _messageBox = null; // Rgpack の rg.msg で表示するため名前はメッセージボックス
        public MessageBoxSubView MessageBox => _messageBox;
        public static string MessageBoxName => "MessageBox";

        [SerializeField] private ElementsSubView _fadeMask = null;
        public IElementsSubView FadeMask => _fadeMask;
        public static string FadeMaskName => "FadeMask";

        [SerializeField] private ElementsSubView _overlay = null;
        public IElementsSubView Overlay => _overlay;
        public static string OverlayName => "Overlay";

        [SerializeField] private MessageBoxSubView _speechBox = null; // Rgpack の rg.say で表示するため名前はスピーチボックス
        public MessageBoxSubView SpeechBox => _speechBox;
        public static string SpeechBoxName => "SpeechBox";

        [SerializeField] private ElementsSubView _choices = null;
        public IElementsSubView Choices => _choices;
        public static string ChoicesName => "Choices";

        public IReadOnlyDictionary<string, ElementsSubView> SubViews { get; private set; }

        /// <summary>
        /// いずれかの <see cref="ElementsSubView.HasManagerLock"/> が true のとき true を取得する
        /// </summary>
        public bool HasManagerLock
        {
            get
            {
                foreach (var pair in SubViews)
                {
                    if (pair.Value.HasManagerLock) return true;
                }
                return false;
            }
        }

        private bool isInitialized;

        public void CommonInit()
        {
            LUIAssert.NotInitialized(this, isInitialized);
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

            SubViews = new Dictionary<string, ElementsSubView>()
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
