using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Lysionium.Views
{
    [AddComponentMenu("UI/Lysionium/Subviews/LUI Message Box Subview")]
    public class MessageBoxSubview : Subview, IMessageBoxSubview
    {
        [SerializeField] private MessageBox _messageBox = null;
        protected MessageBox MessageBox => _messageBox;

        public bool IsInProgress => _messageBox.IsInProgress;

        [SerializeField] private ViewItem _blocker = null;

        [Space, SerializeField] private Button.ButtonClickedEvent _onClick = null;
        [Space, SerializeField] private StartSpeechEvent _onStartSpeech = null;
        [Space, SerializeField] private EndSpeechEvent _onEndSpeech = null;

        private bool isSpeechingNow;

        private event ListuiEventHandler OnCompleted;

        protected override void CommonInitCore()
        {
            _messageBox.OnReachHiddenLink.AddListener(hiddenLinkId =>
            {
                if (hiddenLinkId != _messageBox.HiddenLinkIdOnEof) return;

                var tempAction = OnCompleted;
                OnCompleted = null;
                tempAction?.Invoke(Manager, Arg);
            });

            if (_blocker != null)
            {
                _blocker.Initialize(this);
            }
        }

        public void SetText(string text, IListuiManager manager, IListuiArg arg, ref ISubviewStateProvider stateProvider)
        {
            _messageBox.Clear();
            SetArg(manager, arg);
            if (_messageBox.VisibleMode == MessageBoxVisibleMode.Typing || !IsVisible)
            {
                // メッセージボックスの表示アニメーションが完了してから文字を表示する
                OnEndAnimation += (_, _) => Append(text);
            }
            else
            {
                Append(text);
            }

            if (_blocker != null)
            {
                _blocker.Bind(new SelectOption("", delegate { _onClick.Invoke(); }));
                _blocker.SetVisible(true, true);
            }
        }

        public void SetTextRaw(StringBuilder stringBuilder, IListuiManager manager, IListuiArg arg, ref ISubviewStateProvider stateProvider)
        {
            _messageBox.Clear();
            SetArg(manager, arg);
            if (_messageBox.VisibleMode == MessageBoxVisibleMode.Typing || !IsVisible)
            {
                // メッセージボックスの表示アニメーションが完了してから文字を表示する
                OnEndAnimation += (_, _) => AppendRaw(stringBuilder);
            }
            else
            {
                AppendRaw(stringBuilder);
            }

            if (_blocker != null)
            {
                _blocker.Bind(new SelectOption("", delegate { _onClick.Invoke(); }));
                _blocker.SetVisible(true, true);
            }
        }

        public void Append(string text)
        {
            _messageBox.Append(Manager.Localize(text));
        }

        public void AppendRaw(StringBuilder stringBuilder)
        {
            _messageBox.Append(stringBuilder);
        }

        public void Clear()
        {
            _messageBox.Clear();
        }

        public void DoScheduledAfterCompletion(ListuiEventHandler onEndAnimation)
        {
            OnCompleted += onEndAnimation;
        }

        protected virtual void Update()
        {
            var speechingNow = _messageBox.IsTypingNow;
            if (speechingNow != isSpeechingNow)
            {
                if (speechingNow) { _onStartSpeech.Invoke(); }
                else { _onEndSpeech.Invoke(); }
                isSpeechingNow = speechingNow;
            }
        }

        [System.Serializable] public class AdvanceTextEvent : UnityEvent { }
        [System.Serializable] public class StartSpeechEvent : UnityEvent { }
        [System.Serializable] public class EndSpeechEvent : UnityEvent { }
    }
}
