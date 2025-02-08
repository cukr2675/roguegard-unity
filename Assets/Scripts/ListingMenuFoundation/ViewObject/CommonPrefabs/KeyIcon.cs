using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

namespace ListingMF
{
    [AddComponentMenu("UI/Listing Menu Foundation/LMF Key Icon")]
    [RequireComponent(typeof(LayoutElement))]
    public class KeyIcon : MonoBehaviour
    {
        [SerializeField] private TMP_Text _keyText = null;
        [SerializeField] private Image _keySprite = null;
        [SerializeField] private Vector2 _textMargin = Vector2.zero;

        private LayoutElement layoutElement;
        private RectTransform keySpriteTransform;

        private void Awake()
        {
            layoutElement = GetComponent<LayoutElement>();
            keySpriteTransform = _keySprite.GetComponent<RectTransform>();
            gameObject.SetActive(false);
        }

        public void SetKeyIcon(string keyText, Sprite keySprite)
        {
            _keyText.text = keyText;
            _keyText.ForceMeshUpdate(true, true);
            layoutElement.preferredWidth = _keyText.preferredWidth + _textMargin.x * 2f;
            layoutElement.preferredHeight = _keyText.preferredHeight + _textMargin.y * 2f;
            _keySprite.sprite = keySprite;
            gameObject.SetActive(true);
        }

        public void ClearKeyIcon()
        {
            gameObject.SetActive(false);
        }
    }
}
