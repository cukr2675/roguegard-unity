using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class ChartPadReference : RgpackReference<ChartPadAsset>, IRogueChart
    {
        public CmnReference CurrentCmn { get; private set; }

        public Dictionary<string, string> SerializableTable { get; set; }

        [System.NonSerialized] private IRogueChartSource _source;
        IRogueChartSource IRogueChart.Source => _source ??= new Source(FullId, RgpackId);

        private ChartPadReference(string id, string envRgpackId)
            : base(id, envRgpackId)
        {
            SerializableTable = new Dictionary<string, string>();
        }

        public static IRogueChartSource CreateSource(string id, string envRgpackId)
        {
            return new Source(id, envRgpackId);
        }

        public void MoveNext()
        {
            var nextCmn = Asset.GetNextCmnFrom(CurrentCmn);
            if (nextCmn == null) return; // 終端に達している場合は何もしない

            CurrentCmn = nextCmn.Cmn; // nextFrom() 内で使用するためコモンイベント実行前に設定する
            nextCmn.Invoke();
        }

        [Objforming.Formable]
        private class Source : RgpackReference<ChartPadAsset>, IRogueChartSource
        {
            public CmnReference CurrentCmn { get; set; }

            public Source(string id, string envRgpackId)
                : base(id, envRgpackId)
            {
            }

            public IRogueChart CreateChart()
            {
                return new ChartPadReference(FullId, RgpackId);
            }

            public bool Equals(IRogueChartSource other)
            {
                return other is Source reference && reference.FullId == FullId;
            }

            public override bool Equals(object obj)
            {
                return obj is Source reference && reference.FullId == FullId;
            }

            public override int GetHashCode()
            {
                return FullId.GetHashCode();
            }
        }
    }
}
