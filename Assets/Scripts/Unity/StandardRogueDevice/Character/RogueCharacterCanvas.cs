using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Roguegard;

namespace RoguegardUnity
{
    public class RogueCharacterCanvas : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas = null;
        [SerializeField] private PopNumber _popNumber = null;
        [SerializeField] private HPGauge _hpGauge = null;

        public void Initialize(RogueObj obj)
        {
            _canvas.enabled = false;
            _popNumber.Initialize();
            if (obj != null)
            {
                SetHP(obj);
                _hpGauge.SetVisible(true);
            }
            else
            {
                _hpGauge.SetVisible(false);
            }
        }

        public void Popup(RogueCharacterWork.PopSignType sign, int number, Color color, bool critical)
        {
            _popNumber.Popup(sign, number, color, critical);
            _hpGauge.Damage(number);
        }

        public void SetHP(RogueObj obj)
        {
            var hp = obj.Main.Stats.HP;
            var maxHP = StatsEffectedValues.GetMaxHP(obj);
            _hpGauge.SetHP(hp, maxHP);
        }

        public void UpdateCanvas(RogueObj obj, RogueObj player, int deltaTime)
        {
            _popNumber.UpdateUI(deltaTime);
            _hpGauge.SetColor(player, obj);

            if (obj.HasCollider && !obj.AsTile)
            {
                _canvas.enabled = true;
            }
            else
            {
                _canvas.enabled = false;
            }
        }
    }
}
