using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    public class ChartPadAsset
    {
        private readonly string fullID;

        private readonly PropertiedCmnReference[] cmns;

        public IRogueChartSource ChartSource { get; }

        public ChartPadAsset(ChartPadInfo info, string envRgpackID, string fullID)
        {
            this.fullID = fullID;
            cmns = new PropertiedCmnReference[info.Cmns.Count];
            for (int i = 0; i < info.Cmns.Count; i++)
            {
                cmns[i] = new PropertiedCmnReference(info.Cmns[i], envRgpackID);
            }

            ChartSource = ChartPadReference.CreateSource(fullID, envRgpackID);
        }

        public PropertiedCmnReference GetNextCmnFrom(CmnReference cmn)
        {
            if (cmn == null)
            {
                if (cmns.Length == 0) throw new RogueException($"チャート {fullID} にポイントが一つも存在しません。");

                return cmns[0];
            }

            var cmnIndex = -1;
            for (int i = 0; i < cmns.Length; i++)
            {
                if (cmns[i].Cmn.FullID == cmn.FullID)
                {
                    cmnIndex = i;
                    break;
                }
            }
            if (cmnIndex == -1) throw new RogueException($"イベント ({cmn}) が見つからないため、次のイベントを取得できません。");

            if (cmnIndex < cmns.Length - 1)
            {
                var nextCmn = cmns[cmnIndex + 1];
                return nextCmn;
            }
            else
            {
                return null;
            }
        }
    }
}
