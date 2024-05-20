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

        private readonly RgpackReference[] cmns;

        public Spanning<RgpackReference> Cmns => cmns;

        private RogueChart() { }

        public RogueChart(string id, IEnumerable<RgpackReference> cmns)
        {
            ID = id;
            this.cmns = cmns.ToArray();
        }

        public RogueChart(RogueChart chart)
        {
            ID = chart.ID;
            cmns = chart.cmns.ToArray();
        }

        public void SetRgpackID(string rgpackID)
        {
            for (int i = 0; i < cmns.Length; i++)
            {
                cmns[i] = new RgpackReference(rgpackID, cmns[i].AssetID);
            }
        }

        public RgpackReference GetFirstCmn()
        {
            return cmns[0];
        }

        public RgpackReference GetNextCmnOf(RgpackReference cmn)
        {
            if (cmn == null) throw new System.ArgumentNullException(nameof(cmn));

            var cmnIndex = System.Array.IndexOf(cmns, cmn);
            if (cmnIndex == -1) throw new RogueException($"イベント ({cmn}) が見つからないため、次のイベントを取得できません。");

            if (cmnIndex < cmns.Length - 1)
            {
                var nextCmn = cmns[cmnIndex];
                return nextCmn;
            }
            else
            {
                return null;
            }
        }
    }
}
