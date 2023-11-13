using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoguegardUnity
{
    public abstract class MenuWindow : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;

        public bool IsShow => _canvasGroup.blocksRaycasts;

        public void Show(bool show)
        {
            if (show)
            {
                _canvasGroup.alpha = 1f;
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }
            else
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            }
        }
    }
}
