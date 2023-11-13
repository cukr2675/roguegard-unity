using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Roguegard;

namespace RoguegardUnity
{
    public class PopNumber : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private Image _image10 = null;
        [SerializeField] private Image _image1 = null;
        [SerializeField] private Image _imagePlus = null;
        [SerializeField] private Image _imageCritical = null;
        [SerializeField] private Sprite[] _numberSprites = null;
        [SerializeField] private Sprite _clearSprite = null;

        private int lifeTime = lifeTimeLength;
        private const int lifeTimeLength = 30;

        public void Initialize()
        {
            _canvasGroup.alpha = 0f;
            enabled = false;
        }

        public void UpdateUI(int deltaTime)
        {
            var speed = 16f / RoguegardSettings.PixelPerUnit / 60f;
            transform.localPosition = Vector3.up * (speed * lifeTime);
            lifeTime += deltaTime;
            if (lifeTime >= lifeTimeLength)
            {
                _canvasGroup.alpha = 0f;
                enabled = false;
            }
        }

        public void Popup(RogueCharacterWork.PopSignType sign, int number, Color color, bool critical)
        {
            if (sign == RogueCharacterWork.PopSignType.Clear) return;

            if (number < 100)
            {
                // 0 ~ 99
                if (number / 10 == 0)
                {
                    // 一桁
                    _image10.sprite = _clearSprite;
                    _image1.rectTransform.localPosition = Vector3.zero;
                }
                else
                {
                    // 二桁
                    _image10.sprite = _numberSprites[number / 10];
                    var width = _image10.rectTransform.rect.width;
                    _image10.rectTransform.localPosition = Vector3.left * (width / 2f);
                    _image1.rectTransform.localPosition = Vector3.right * (width / 2f - 1f / RoguegardSettings.PixelPerUnit);
                }
                _image1.sprite = _numberSprites[number % 10];
                _imagePlus.enabled = false;
            }
            else
            {
                // 99+
                _image10.sprite = _numberSprites[9];
                _image1.sprite = _numberSprites[9];
                _imagePlus.enabled = true;
            }
            _image10.color = color;
            _image1.color = color;
            _imagePlus.color = color;

            _imageCritical.enabled = critical;
            transform.localPosition = Vector3.zero;
            _canvasGroup.alpha = 1f;
            lifeTime = 0;
            enabled = true;
        }
    }
}
