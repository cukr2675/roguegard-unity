using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Events;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/Sub Views/LMF Message Box Sub View")]
    public class MessageBoxSubView : ElementsSubView
    {
        [SerializeField] private MessageBox _messageBox = null;
        public MessageBox MessageBox => _messageBox;

        [SerializeField] private ViewElement _blocker = null;

        [Space, SerializeField] private Button.ButtonClickedEvent _onClickWithoutBlock = null;
        [Space, SerializeField] private StartSpeechEvent _onStartSpeech = null;
        [Space, SerializeField] private EndSpeechEvent _onEndSpeech = null;

        private bool isInitialized;
        private bool isSpeechingNow;

        private event HandleEndAnimation OnCompleted;

        public void Initialize()
        {
            LMFAssert.NotInitialized(this, isInitialized);
            isInitialized = true;

            _messageBox.OnReachHiddenLink.AddListener(hiddenLinkID =>
            {
                if (hiddenLinkID != _messageBox.HiddenLinkIDOnEOF) return;

                OnCompleted?.Invoke(Manager, Arg);
                OnCompleted = null;
            });

            if (_blocker != null)
            {
                _blocker.Initialize(this);
                _blocker.SetElement(
                    SelectOptionHandler.Instance, SelectOption.Create<IListMenuManager, IListMenuArg>("", delegate { _onClickWithoutBlock.Invoke(); }));
                _blocker.SetBlock(false);
                _blocker.SetVisible(true, true);
            }
        }

        public override void SetParameters(
            IReadOnlyList<object> list, IElementHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref IElementsSubViewStateProvider stateProvider)
        {
            _messageBox.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                var name = handler.GetName(list[i], manager, arg);
                name = manager.Localize(name);
                _messageBox.Append(name);
            }
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
