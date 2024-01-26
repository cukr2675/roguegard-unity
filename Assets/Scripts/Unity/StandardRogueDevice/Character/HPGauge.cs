using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Roguegard;

namespace RoguegardUnity
{
    public class HPGauge : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup = null;
        [SerializeField] private Image _gauge = null;
        [SerializeField] private Image _gaugeBackground = null;
        [SerializeField] private Image _image1000 = null;
        [SerializeField] private Image _image100 = null;
        [SerializeField] private Image _image10 = null;
        [SerializeField] private Image _image1 = null;
        [SerializeField] private Sprite[] _numberSprites = null;
        [SerializeField] private Sprite _clearSprite = null;

        private int hp;
        private int maxHP;

        public void SetVisible(bool visible)
        {
            _canvasGroup.alpha = visible ? 1f : 0f;
        }

        public void SetColor(RogueObj player, RogueObj obj)
        {
            if (StatsEffectedValues.AreVS(player, obj))
            {
                // 敵
                _gauge.color = (Color)new Color32(255, 255, 0, 255) * RoguegardSettings.LightRatio;
                _gaugeBackground.color = (Color)new Color32(255, 0, 0, 255) * RoguegardSettings.LightRatio;
                SetVisible(true);
                return;
            }

            if (obj == player)
            {
                // プレイヤー
                SetVisible(false);
            }
            else if (RogueParty.Equals(obj, player))
            {
                // 味方
                _gauge.color = (Color)new Color32(196, 255, 0, 255) * RoguegardSettings.LightRatio;
                _gaugeBackground.color = (Color)new Color32(255, 64, 0, 255) * RoguegardSettings.LightRatio;
                SetVisible(true);
            }
            else
            {
                // 中立
                SetVisible(false);
            }
        }

        public void Damage(int damage)
        {
            hp -= damage;
            SetHP(hp, maxHP);
        }

        public void SetHP(int hp, int maxHP)
        {
            hp = Mathf.Max(hp, 0);
            maxHP = Mathf.Max(maxHP, 0);

            this.hp = hp;
            this.maxHP = maxHP;
            if (maxHP == 0f)
            {
                // ゼロ除算対策
                _gauge.rectTransform.anchorMax = new Vector2(0f, 1f);
            }
            else
            {
                _gauge.rectTransform.anchorMax = new Vector2((float)hp / maxHP, 1f);
            }

            if (hp < 10000)
            {
                // 0 ~ 99
                if (hp / 1000 != 0)
                {
                    // 四桁
                    _image1000.sprite = _numberSprites[hp / 1000];
                    _image100.sprite = _numberSprites[hp / 100 % 10];
                    _image10.sprite = _numberSprites[hp / 10 % 10];
                    _image1.sprite = _numberSprites[hp / 1 % 10];
                }
                if (hp / 100 != 0)
                {
                    // 三桁
                    _image1000.sprite = _clearSprite;
                    _image100.sprite = _numberSprites[hp / 100 % 10];
                    _image10.sprite = _numberSprites[hp / 10 % 10];
                    _image1.sprite = _numberSprites[hp / 1 % 10];
                }
                if (hp / 10 != 0)
                {
                    // 二桁
                    _image1000.sprite = _clearSprite;
                    _image100.sprite = _clearSprite;
                    _image10.sprite = _numberSprites[hp / 10 % 10];
                    _image1.sprite = _numberSprites[hp / 1 % 10];
                }
                else
                {
                    // 一桁
                    _image1000.sprite = _clearSprite;
                    _image100.sprite = _clearSprite;
                    _image10.sprite = _clearSprite;
                    _image1.sprite = _numberSprites[hp / 1 % 10];
                }
            }
            else
            {
                // 9999+
                _image1000.sprite = _numberSprites[9];
                _image100.sprite = _numberSprites[9];
                _image10.sprite = _numberSprites[9];
                _image1.sprite = _numberSprites[9];
            }
        }
    }
}
