using System.Collections.Generic;
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

        private event ListMenuEventHandler OnCompleted;

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

        public override void SetParameters(
            IReadOnlyList<object> list, IViewItemHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref ISubviewStateProvider stateProvider)
        {
            _messageBox.Clear();
            OnEndAnimation += (manager, arg) =>
            {
                // メッセージボックスの表示アニメーションが完了してから文字を表示する
                for (int i = 0; i < list.Count; i++)
                {
                    var name = handler.GetName(list[i], manager, arg);
                    name = manager.Localize(name);
                    _messageBox.Append(name);
                }
            };
            SetArg(manager, arg);
            SetStatusCode(0);

            if (_blocker != null)
            {
                _blocker.Bind(
                    SelectOption.Create<IListMenuManager, IListMenuArg>("", delegate { _onClick.Invoke(); }), SelectOptionViewItemHandler.Instance);
                _blocker.SetVisible(true, true);
            }
        }

        public void Append(string text)
        {
            _messageBox.Append(text);
        }

        public void Append(int integer)
        {
            _messageBox.Append(integer);
        }

        public void Append(float number)
        {
            _messageBox.Append(number);
        }

        public void Clear()
        {
            _messageBox.Clear();
        }

        public void DoScheduledAfterCompletion(ListMenuEventHandler onEndAnimation)
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
