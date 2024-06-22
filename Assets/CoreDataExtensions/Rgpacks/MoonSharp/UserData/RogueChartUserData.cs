using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoonSharp.Interpreter;

namespace Roguegard.Rgpacks.MoonSharp
{
    [MoonSharpUserData]
    public class RogueChartUserData
    {
        private readonly IRogueChartSource chartSource;

        public RogueChartUserData(IRogueChartSource chartSource)
        {
            this.chartSource = chartSource;
        }

        public void nextFrom(string chartCmnID)
        {
            //var envRgpackID = executionContext.CurrentGlobalEnv.Get("__rgpack").String;
            var envRgpackID = "Playtest";
            var argumentCmn = new CmnReference(chartCmnID, envRgpackID);
            var worldInfo = RogueWorldInfo.GetByCharacter(RogueDevice.Primary.Player);

            if (worldInfo.ChartState.TryGet<ChartPadReference>(chartSource, out var chartReference) &&
                chartReference.CurrentCmn.FullID != argumentCmn.FullID)
            {
                Debug.LogError($"�w�肳�ꂽ Cmn ({argumentCmn.FullID}) �����݂� Cmn ({chartReference.CurrentCmn?.FullID}) �ƈ�v���܂���B");
                return;
            }

            worldInfo.ChartState.PushNext(chartSource);
        }
    }
}
