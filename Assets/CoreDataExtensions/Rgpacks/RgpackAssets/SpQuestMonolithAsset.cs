using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    public class SpQuestMonolithAsset
    {
        public IRogueChartSource MainChartSource { get; }

        public SpQuestMonolithAsset(SpQuestMonolithInfo info, string envRgpackID)
        {
            MainChartSource = ChartPadReference.CreateSource(info.MainChart, envRgpackID);
        }
    }
}
