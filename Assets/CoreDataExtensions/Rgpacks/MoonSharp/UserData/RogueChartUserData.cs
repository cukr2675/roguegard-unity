using MoonSharp.Interpreter;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Roguegard.Rgpacks.MoonSharp
{
    [MoonSharpUserData]
    [SuppressMessage("Style", "IDE1006")]
    public class RogueChartUserData
    {
        private readonly IRogueChartSource chartSource;

        public RogueChartUserData(IRogueChartSource chartSource)
        {
            this.chartSource = chartSource;
        }

        public void nextFrom(string id, Script ownerScript)
        {
            var envRgpackId = ownerScript.DoString("return __rgpack").String;
            envRgpackId ??= "Playtest";
            var argumentCmn = new CmnReference(id, envRgpackId);
            var worldInfo = RogueWorldInfo.GetByCharacter(RogueDevice.Primary.Player);

            if (worldInfo.ChartState.TryGet<ChartPadReference>(chartSource, out var chartReference) &&
                chartReference.CurrentCmn.FullId != argumentCmn.FullId)
            {
                Debug.LogError($"指定された Cmn ({argumentCmn.FullId}) が現在の Cmn ({chartReference.CurrentCmn?.FullId}) と一致しません。");
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
