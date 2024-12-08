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

        public RogueChartUserData(IRogueChartSource chartSource, Script ownerScript)
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
                Debug.LogError($"指定された Cmn ({argumentCmn.FullID}) が現在の Cmn ({chartReference.CurrentCmn?.FullID}) と一致しません。");
                return;
            }

            worldInfo.ChartState.PushNext(chartSource);
        }

        public void setS(string key, string value)
        {
            var worldInfo = RogueWorldInfo.GetByCharacter(RogueDevice.Primary.Player);
            if (worldInfo.ChartState.TryGet<ChartPadReference>(chartSource, out var chartReference, true))
            {
                chartReference.SerializableTable[key] = value;
            }
        }

        public string getS(string key)
        {
            var worldInfo = RogueWorldInfo.GetByCharacter(RogueDevice.Primary.Player);
            if (worldInfo.ChartState.TryGet<ChartPadReference>(chartSource, out var chartReference, true) &&
                chartReference.SerializableTable.TryGetValue(key, out var value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }
    }
}
