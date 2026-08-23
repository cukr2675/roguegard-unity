using UnityEngine;
using UnityEngine.EventSystems;

namespace Lysionium.Views
{
    public class ClickEventGestureDefinition : EventGestureDefinition
    {
        [SerializeField] private int _clickCount;
        [SerializeField] private float _clickDelayTime;

        private string eventGestureName;

        protected virtual void Awake()
        {
            eventGestureName = base.name;
        }

        public override IEventGestureDefinitionRecognizer CreateDefinitionRecognizer()
        {
            return new DefinitionRecognizer { definition = this };
        }

        private class DefinitionRecognizer :
            IEventGestureDefinitionRecognizer,
            IEventGestureDefinitionRecognizerHandler,
            IEventGestureDefinitionRecognizerHandler<PointerEventData>,
            IEventGestureDefinitionRecognizerHandler<BaseEventData>
        {
            public ClickEventGestureDefinition definition;
            private int currentClickCount;
            private float currentClickDelayTimeLeft;

            bool IEventGestureDefinitionRecognizer.IsIndependent => false;

            float IEventGestureDefinitionRecognizer.GestureProgress => 0f;

            private void AddClickCount(EventGestureRecognizer recognizer)
            {
                currentClickCount++;
                currentClickDelayTimeLeft = definition._clickDelayTime;

                if (currentClickCount >= definition._clickCount)
                {
                    recognizer.ConfirmEventGesture(definition.eventGestureName);
                }
            }

            void IEventGestureDefinitionRecognizerHandler.Handle(
                string eventName, EventGestureRecognizer recognizer)
            {
                if (eventName == "Submit")
                {
                    AddClickCount(recognizer);
                }
            }

            void IEventGestureDefinitionRecognizerHandler<PointerEventData>.Handle(
                string eventName, EventGestureRecognizer recognizer, PointerEventData eventData)
            {
                if (eventName == "PointerClick")
                {
                    AddClickCount(recognizer);
                }
            }

            void IEventGestureDefinitionRecognizerHandler<BaseEventData>.Handle(
                string eventName, EventGestureRecognizer recognizer, BaseEventData eventData)
            {
                if (eventName == "Submit")
                {
                    AddClickCount(recognizer);
                }
            }

            void IEventGestureDefinitionRecognizer.ResetRecognizer()
            {
                currentClickCount = 0;
                currentClickDelayTimeLeft = 0f;
            }

            void IEventGestureDefinitionRecognizer.UpdateRecognizer(EventGestureRecognizer recognizer)
            {
                if (currentClickDelayTimeLeft > 0f)
                {
                    currentClickDelayTimeLeft -= Time.deltaTime;
                    if (currentClickDelayTimeLeft <= 0f)
                    {
                        recognizer.DenyEventGesture(definition.eventGestureName);
                    }
                }
            }
        }
    }
}
