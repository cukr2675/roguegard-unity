using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class RogueChartState
    {
        private readonly List<Item> items = new();
        private readonly List<RgpackReference> nexts = new();

        public void MoveNext(RgpackReference chart)
        {
            nexts.Add(chart);
        }

        public bool TryUpdate()
        {
            if (nexts.Count == 0) return false;

            var chartReference = nexts[0];
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (!RgpackReference.Equals(item.ChartReference, chartReference)) continue;

                var nextEventRef = item.Chart.GetNextEventOf(item.CurrentCmn);
                var nextEvent = nextEventRef.GetData<IScriptingEvent>();
                nextEvent.Invoke();
                item.CurrentCmn = nextEventRef;
                return true;
            }
            {
                var item = new Item(chartReference);
                items.Add(item);
                var eventRef = item.Chart.GetFirstEvent();
                var ev = eventRef.GetData<IScriptingEvent>();
                ev.Invoke();
                item.CurrentCmn = eventRef;
            }

            nexts.RemoveAt(0);
            return true;
        }

        [Objforming.Formable]
        private class Item
        {
            public RgpackReference ChartReference { get; }
            public RgpackReference CurrentCmn { get; set; }

            [System.NonSerialized] private RogueChart _chart;
            public RogueChart Chart => _chart ??= ChartReference.GetData<RogueChart>();

            public Item(RgpackReference chartReference)
            {
                ChartReference = chartReference;
            }
        }
    }
}
