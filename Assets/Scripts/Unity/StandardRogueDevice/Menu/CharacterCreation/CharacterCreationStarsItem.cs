using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

namespace RoguegardUnity
{
    public class CharacterCreationStarsItem : MonoBehaviour
    {
        [SerializeField] private Image _starPrefab = null;
        [SerializeField] private TMP_Text _number = null;
        [SerializeField] private Image _numberStar = null;
        [Space]
        [SerializeField] private Sprite _normalStar = null;
        [SerializeField] private Sprite _unknownStar = null;

        private Image[] stars;

        public void SetStars(int count, bool costIsUnknown)
        {
            if (stars == null)
            {
                stars = new Image[5];
                for (int i = 0; i < stars.Length; i++)
                {
                    var star = Instantiate(_starPrefab, transform);
                    star.enabled = false;
                    stars[i] = star;
                }
            }

            // ¯‚Ì•\Ž¦”‚ÆˆÊ’u‚ðÝ’è
            if (count <= 5)
            {
                var thisWidth = ((RectTransform)transform).rect.width;
                for (int i = 0; i < stars.Length; i++)
                {
                    var star = stars[i];
                    star.sprite = costIsUnknown ? _unknownStar : _normalStar;
                    star.enabled = i < count;
                    var position = star.rectTransform.localPosition;
                    position.x = thisWidth * i / count;
                    star.rectTransform.localPosition = position;
                }
                _number.enabled = false;
                _numberStar.enabled = false;
            }
            else
            {
                {
                    _number.SetText(count.ToString());
                    _number.enabled = true;
                    _numberStar.sprite = costIsUnknown ? _unknownStar : _normalStar;
                    _numberStar.enabled = true;
                }
                for (int i = 0; i < stars.Length; i++)
                {
                    var star = stars[i];
                    star.enabled = false;
                }
            }
        }
    }
}
