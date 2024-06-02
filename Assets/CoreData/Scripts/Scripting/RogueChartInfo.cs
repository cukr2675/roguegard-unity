using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public static class RogueChartInfo
    {
        public static RogueChart GetChart(RogueObj obj)
        {
            if (obj.TryGet<Info>(out var info))
            {
                return info.chart;
            }
            else
            {
                return null;
            }
        }

        public static void SetChartTo(RogueObj obj, RogueChart chart)
        {
            if (chart == null) throw new System.ArgumentNullException(nameof(chart));

            if (!obj.TryGet<Info>(out var info))
            {
                info = new Info();
                obj.SetInfo(info);
            }

            info.chart = chart;
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public RogueChart chart;

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo other)
            {
                return false;
            }

            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
            {
                return new Info() { chart = chart };
            }

            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj)
            {
                return this;
            }
        }
    }
}
