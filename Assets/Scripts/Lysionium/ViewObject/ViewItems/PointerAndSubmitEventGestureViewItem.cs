using UnityEngine.EventSystems;

namespace Lysionium.Views
{
    public abstract class PointerAndSubmitEventGestureViewItem :
        EventGestureViewItem,
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,
        ISubmitHandler
    {
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            EventGestureRecognizer.Handle("PointerEnter", eventData);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            EventGestureRecognizer.Handle("PointerExit", eventData);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            EventGestureRecognizer.SetTarget(this);
            EventGestureRecognizer.Handle("PointerDown", eventData);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            EventGestureRecognizer.Handle("PointerUp", eventData);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            EventGestureRecognizer.Handle("PointerClick", eventData);
        }

        void ISubmitHandler.OnSubmit(BaseEventData eventData)
        {
            EventGestureRecognizer.SetTarget(this);
            EventGestureRecognizer.Handle("Submit", eventData);
        }
    }
}
