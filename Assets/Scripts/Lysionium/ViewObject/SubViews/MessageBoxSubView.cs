using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Events;

namespace Lysionium
{
    [AddComponentMenu("UI/Lysionium/Sub Views/LUI Message Box Sub View")]
    public class MessageBoxSubView : ElementsSubView
    {
        [SerializeField] private MessageBox _messageBox = null;
        public MessageBox MessageBox => _messageBox;

        [SerializeField] private ViewElement _blocker = null;

        [Space, SerializeField] private Button.ButtonClickedEvent _onClick = null;
        [Space, SerializeField] private StartSpeechEvent _onStartSpeech = null;
        [Space, SerializeField] private EndSpeechEvent _onEndSpeech = null;

        private bool isSpeechingNow;

        private event HandleEndAnimation OnCompleted;

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
                _blocker.SetElement(
                    SelectOption.Create<IListMenuManager, IListMenuArg>("", delegate { _onClick.Invoke(); }), SelectOptionHandler.Instance);
                _blocker.SetVisible(true, true);
            }
        }

        public override void SetParameters(
            IReadOnlyList<object> list, IElementHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref IElementsSubViewStateProvider stateProvider)
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
        }

        public void DoScheduledAfterCompletion(HandleEndAnimation onEndAnimation)
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
