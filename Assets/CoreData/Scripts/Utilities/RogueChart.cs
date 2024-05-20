using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard
{
    [Objforming.Formable]
    public class RogueChart
    {
        public string ID { get; }

        private readonly RgpackReference[] evs;

        public Spanning<RgpackReference> Events => evs;

        private RogueChart() { }

        public RogueChart(string id, IEnumerable<RgpackReference> evs)
        {
            ID = id;
            this.evs = evs.ToArray();
        }

        public RogueChart(RogueChart chart)
        {
            ID = chart.ID;
            evs = chart.evs.ToArray();
        }

        public void SetRgpackID(string rgpackID)
        {
            for (int i = 0; i < evs.Length; i++)
            {
                evs[i] = new RgpackReference(rgpackID, evs[i].AssetID);
            }
        }

        public RgpackReference GetFirstEvent()
        {
            return evs[0];
        }

        public RgpackReference GetNextEventOf(RgpackReference ev)
        {
            if (ev == null) throw new System.ArgumentNullException(nameof(ev));

            var evIndex = System.Array.IndexOf(evs, ev);
            if (evIndex == -1) throw new RogueException($"イベント ({ev}) が見つからないため、次のイベントを取得できません。");

            if (evIndex < evs.Length - 1)
            {
                var nextEv = evs[evIndex];
                return nextEv;
            }
            else
            {
                return null;
            }
        }
    }
}
