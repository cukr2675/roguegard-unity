using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using TMPro;
using Roguegard;
using Roguegard.CharacterCreation;

namespace RoguegardUnity
{
    public class StatsWindow : MenuWindow
    {
        [SerializeField] private TMP_Text _text = null;
        [SerializeField] private TMP_Text _hpText = null;
        [SerializeField] private TMP_Text _mpText = null;
        [SerializeField] private TMP_Text _dungeonText = null;

        private static readonly StringBuilder textBuilder = new StringBuilder();
        private static readonly StringBuilder hpTextBuilder = new StringBuilder();
        private static readonly StringBuilder mpTextBuilder = new StringBuilder();
        private static readonly RogueNameBuilder nameBuilder = new RogueNameBuilder();

        public void SetText(RogueObj obj)
        {
            var mainStats = obj.Main.Stats;
            textBuilder.Clear();
            hpTextBuilder.Clear();
            mpTextBuilder.Clear();

            textBuilder.AppendLine($"満腹度：{mainStats.Nutrition} / {StatsEffectedValues.GetMaxNutrition(obj)}");
            hpTextBuilder.AppendLine();
            var totalMoney = 0;
            var spaceObjs = obj.Space.Objs;
            for (int i = 0; i < spaceObjs.Count; i++)
            {
                var spaceObj = spaceObjs[i];
                if (spaceObj == null || spaceObj.Main.InfoSet != RoguegardSettings.MoneyInfoSet) continue;

                totalMoney += spaceObj.Stack;
            }
            mpTextBuilder.AppendLine($"{totalMoney} G");

            textBuilder.AppendLine();
            hpTextBuilder.AppendLine("HP");
            mpTextBuilder.AppendLine("MP");

            obj.GetName(nameBuilder);
            StandardRogueDeviceUtility.Localize(nameBuilder);
            textBuilder.AppendLine(nameBuilder.ToString());
            hpTextBuilder.Append(mainStats.HP).Append(" / ").Append(StatsEffectedValues.GetMaxHP(obj)).AppendLine();
            mpTextBuilder.Append(mainStats.MP).Append(" / ").Append(StatsEffectedValues.GetMaxMP(obj)).AppendLine();

            _text.SetText(textBuilder);
            _hpText.SetText(hpTextBuilder);
            _mpText.SetText(mpTextBuilder);
        }

        public void SetDungeon(RogueObj dungeon)
        {
            dungeon.GetName(nameBuilder);
            StandardRogueDeviceUtility.Localize(nameBuilder);
            if (DungeonInfo.TryGet(dungeon, out var info))
            {
                var levelText = info.GetLevelText(dungeon);
                _dungeonText.SetText($"{nameBuilder} {levelText}");
            }
            else
            {
                var levelText = DungeonInfo.GetLevelText(DungeonLevelType.None, dungeon.Main.Stats.Lv);
                _dungeonText.SetText($"{nameBuilder} {levelText}");
            }
        }
    }
}
