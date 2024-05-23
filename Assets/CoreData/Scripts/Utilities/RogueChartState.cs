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

        public RgpackReference GetCurrentCmn(RgpackReference chartReference)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (!RgpackReference.Equals(item.ChartReference, chartReference)) continue;

                return item.CurrentCmn;
            }
            return null;
        }

        public bool TryUpdate()
        {
            if (nexts.Count == 0) return false;

            var chartReference = nexts[0];
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (!RgpackReference.Equals(item.ChartReference, chartReference)) continue;

                var nextCmnRef = item.Chart.GetNextCmnOf(item.CurrentCmn);
                var nextCmn = nextCmnRef.GetData<IScriptingCmn>();
                nextCmn.Invoke();
                item.CurrentCmn = nextCmnRef;
                return true;
            }
            {
                var item = new Item(chartReference);
                items.Add(item);
                var cmnRef = item.Chart.GetFirstCmn();
                var cmn = cmnRef.GetData<IScriptingCmn>();
                cmn.Invoke();
                item.CurrentCmn = cmnRef;
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
