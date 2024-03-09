using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RuntimeDotter
{
    public class DotterHoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Image _target = null;
        [SerializeField] private ColorBlock _color = ColorBlock.defaultColorBlock;
        [SerializeField] private KeyCode _keyCode = KeyCode.Space;

        private bool pointerIsDown;

        public bool IsDown { get; private set; }

        private void Update()
        {
            var keyIsDown = Input.GetKey(_keyCode);

            IsDown = keyIsDown || pointerIsDown;
            _target.color = IsDown ? _color.pressedColor : _color.normalColor;
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            pointerIsDown = true;
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            pointerIsDown = false;
        }
    }
}
