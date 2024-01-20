using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

namespace RoguegardUnity
{
    public class PaintButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool IsDown { get; private set; }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            IsDown = true;
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            IsDown = false;
        }
    }
}
