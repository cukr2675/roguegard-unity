using Roguegard;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoguegardUnity
{
    public class AnchorKeyItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _image = null;

        public event AnchorButton.EnterItemHandler OnEnter;

        private void Start()
        {
            _image.color = RoguegardSettings.White;
        }

        public void SetSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            OnEnter?.Invoke(this);
            _image.color = RoguegardSettings.Gray;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            _image.color = RoguegardSettings.White;
        }
    }
}
