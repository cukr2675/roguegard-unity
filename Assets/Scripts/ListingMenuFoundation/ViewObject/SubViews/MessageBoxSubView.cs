using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/Sub Views/LMF Message Box Sub View")]
    public class MessageBoxSubView : ElementsSubView
    {
        [SerializeField] private MessageBox _messageBox = null;
        public MessageBox MessageBox => _messageBox;

        [Space, SerializeField] private UnityEvent _onStartSpeech = null;
        [Space, SerializeField] private UnityEvent _onEndSpeech = null;

        private readonly Queue<string> subViewNamesAfterCompletion = new();

        private bool isInitialized;
        private bool isSpeechingNow;

        private event HandleEndAnimationAction OnCompleted;

        public void Initialize()
        {
            LMFAssert.NotInitialized(this, isInitialized);
            isInitialized = true;

            _messageBox.OnReachHiddenLink.AddListener(hiddenLinkID =>
            {
                if (hiddenLinkID != _messageBox.HiddenLinkIDOnEOF) return;

                while (subViewNamesAfterCompletion.TryDequeue(out var subViewName))
                {
                    Manager?.GetSubView(subViewName).Show();
                }
                OnCompleted?.Invoke(Manager, Arg);
                OnCompleted = null;
            });
        }

        public override void SetParameters(
            IReadOnlyList<object> list, IElementHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref IElementsSubViewStateProvider stateProvider)
        {
            _messageBox.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                var name = handler.GetName(list[i], manager, arg);
                _messageBox.Append(name);
            }
            SetArg(manager, arg);
            SetStatusCode(0);
        }

        public void ShowScheduledAfterCompletion(string subViewName)
        {
            subViewNamesAfterCompletion.Enqueue(subViewName);
        }

        public void ShowScheduledAfterCompletion(HandleEndAnimationAction handleEndAnimation)
        {
            OnCompleted += handleEndAnimation;
        }

        private void Update()
        {
            var speechingNow = _messageBox.IsTypingNow;
            if (speechingNow != isSpeechingNow)
            {
                if (speechingNow) { _onStartSpeech.Invoke(); }
                else { _onEndSpeech.Invoke(); }
                isSpeechingNow = speechingNow;
            }
        }
    }
}
