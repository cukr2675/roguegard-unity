using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    public class ChartPadAsset
    {
        private readonly string fullId;

        private readonly PropertiedCmnReference[] cmns;

        public IRogueChartSource ChartSource { get; }

        public ChartPadAsset(ChartPadInfo info, string envRgpackId, string fullId)
        {
            this.fullId = fullId;
            cmns = new PropertiedCmnReference[info.Cmns.Length];
            for (int i = 0; i < info.Cmns.Length; i++)
            {
                cmns[i] = info.Cmns[i].ToReference(envRgpackId);
            }

            ChartSource = ChartPadReference.CreateSource(fullId, envRgpackId);
        }

        public PropertiedCmnReference GetNextCmnFrom(CmnReference cmn)
        {
            if (cmn == null)
            {
                if (cmns.Length == 0) return null;

                return cmns[0];
            }

            var cmnIndex = -1;
            for (int i = 0; i < cmns.Length; i++)
            {
                if (cmns[i].Cmn.FullId == cmn.FullId)
                {
                    cmnIndex = i;
                    break;
                }
            }
            if (cmnIndex == -1) throw new RogueException($"コモンイベント ({cmn}) が見つからないため、次のイベントを取得できません。");

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
