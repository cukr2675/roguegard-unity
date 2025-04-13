using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    public class ScenarioMonolithAsset
    {
        public IRogueChartSource MainChartSource { get; }

        public ScenarioMonolithAsset(ScenarioMonolithInfo info, string envRgpackId)
        {
            MainChartSource = ChartPadReference.CreateSource(info.MainChart, envRgpackId);
        }
    }
}
