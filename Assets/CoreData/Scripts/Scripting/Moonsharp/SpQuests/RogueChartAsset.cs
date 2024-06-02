using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoonSharp.Interpreter;

namespace Roguegard.Scripting.MoonSharp
{
    [MoonSharpUserData]
    public class RogueChartAsset
    {
        private readonly RgpackReference chart;

        public RogueChartAsset(RgpackReference chart)
        {
            this.chart = chart;
        }

        public void nextFrom(string chartCmnID)
        {
            //var envRgpackID = executionContext.CurrentGlobalEnv.Get("__rgpack").String;
            var envRgpackID = "Playtest";
            var reference = new RgpackReference(chartCmnID);
            reference.LoadFullID(envRgpackID);
            var worldInfo = RogueWorldInfo.GetByCharacter(RogueDevice.Primary.Player);
            var currentCmn = worldInfo.ChartState.GetCurrentCmn(chart);

            if (!RgpackReference.Equals(currentCmn, reference))
            {
                Debug.LogError($"�w�肳�ꂽ Cmn ({reference?.ID}) �����݂� Cmn ({currentCmn?.ID}) �ƈ�v���܂���B");
                return;
            }

            worldInfo.ChartState.MoveNext(chart);
        }
    }
}
