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
        [SerializeField] private HpGauge _hpGauge = null;

        public void Initialize(RogueObj obj)
        {
            _canvas.enabled = false;
            _popNumber.Initialize();
            if (obj != null)
            {
                SetHp(obj);
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

        public void SetHp(RogueObj obj)
        {
            var hp = obj.Main.Stats.Hp;
            var maxHp = StatsEffectedValues.GetMaxHp(obj);
            _hpGauge.SetHp(hp, maxHp);
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
