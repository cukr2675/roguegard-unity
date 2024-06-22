using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class RogueChartState
    {
        private readonly List<IRogueChart> charts = new();
        private readonly List<IRogueChartSource> nextCharts = new();

        public void PushNext(IRogueChartSource source)
        {
            nextCharts.Add(source);
        }

        public bool TryGet<T>(IRogueChartSource source, out T chart)
            where T : IRogueChart
        {
            for (int i = 0; i < charts.Count; i++)
            {
                var item = charts[i];
                if (!item.Source.Equals(source)) continue;

                if (item is T t)
                {
                    chart = t;
                    return true;
                }
                else
                {
                    break;
                }
            }
            chart = default;
            return false;
        }

        public bool TryUpdate()
        {
            if (nextCharts.Count == 0) return false;

            var chartSource = nextCharts[0];
            nextCharts.RemoveAt(0);
            for (int i = 0; i < charts.Count; i++)
            {
                var chart = charts[i];
                if (!chart.Source.Equals(chartSource)) continue;

                chart.MoveNext();
                return true;
            }
            {
                var chart = chartSource.CreateChart();
                charts.Add(chart);
                chart.MoveNext();
                return true;
            }
        }
    }
}
