using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

namespace RoguegardUnity
{
    public class PaintButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
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

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            // PaintMenuView �� PointerDrag �ɐݒ肳��邽�߁A�󃁃\�b�h����������B
        }
    }
}
