using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using Roguegard;

namespace RoguegardUnity
{
    public class AnchorBalloon : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private Image _background = null;
        [SerializeField] private Image _image = null;

        public bool IsClick { get; private set; }

        private bool readyToClick;

        private void Down()
        {
            readyToClick = true;

            _background.color = _image.color = RoguegardSettings.Gray;
        }

        private void Up()
        {
            if (readyToClick)
            {
                IsClick = true;
                readyToClick = false;
            }

            _background.color = _image.color = RoguegardSettings.White;
        }

        public void CommandUpdate()
        {
            IsClick = false;
        }

        public void SetEnabled(bool enabled, Sprite icon)
        {
            Up();
            _canvasGroup.alpha = enabled ? 1f : 0f;
            _canvasGroup.interactable = enabled;
            _canvasGroup.blocksRaycasts = enabled;
            _image.sprite = icon;
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            Down();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            Up();
        }
    }
}
