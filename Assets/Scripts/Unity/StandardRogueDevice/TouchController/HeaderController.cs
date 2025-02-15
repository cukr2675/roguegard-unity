using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using UnityEngine.UI;
using TMPro;
using Roguegard;

namespace RoguegardUnity
{
    public class HeaderController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text0 = null;
        [SerializeField] private TMP_Text _text1 = null;
        [SerializeField] private Image _hpBackground1 = null;
        [SerializeField] private Image _hpBackground2 = null;
        [SerializeField] private Image _hpGauge = null;
        [SerializeField] private TMP_Text _text2 = null;
        [SerializeField] private TMP_Text _text3 = null;

        private readonly StringBuilder textBuilder = new StringBuilder();

        public void Initialize()
        {
            //_hpGauge.color = 
            gameObject.SetActive(true);
        }

        public void UpdateHeader(RogueObj player)
        {
            var dungeon = player.Location;
            var stats = player.Main.Stats;
            textBuilder.Clear();
            if (dungeon != null)
            {
                textBuilder.Append("B").Append(dungeon.Main.Stats.Lv).Append("F Lv");
            }
            else
            {
                textBuilder.Append("-F Lv");
            }
            textBuilder.Append(stats.Lv).Append(" HP");
            _text0.SetText(textBuilder);

            textBuilder.Clear();
            var maxHp = StatsEffectedValues.GetMaxHp(player);
            textBuilder.Append(stats.Hp).Append("/").Append(maxHp);
            _text1.SetText(textBuilder);

            _hpBackground1.fillAmount = Mathf.Min((float)maxHp / 100, .1f);
            _hpBackground2.fillAmount = _hpBackground1.fillAmount;
            _hpGauge.fillAmount = (float)stats.Hp / maxHp * _hpBackground1.fillAmount;

            _text2.SetText("MP ");

            textBuilder.Clear();
            textBuilder.Append(stats.Mp).Append("/").Append(StatsEffectedValues.GetMaxMp(player));
            _text3.SetText(textBuilder);
        }
    }
}
