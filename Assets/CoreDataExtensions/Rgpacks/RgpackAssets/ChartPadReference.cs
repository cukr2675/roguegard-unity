using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class ChartPadReference : RgpackReference<ChartPadAsset>, IRogueChart
    {
        public CmnReference CurrentCmn { get; private set; }

        [System.NonSerialized] private IRogueChartSource _source;
        IRogueChartSource IRogueChart.Source => _source ??= new Source(FullID, RgpackID);

        private ChartPadReference(string id, string envRgpackID)
            : base(id, envRgpackID)
        {
        }

        public static IRogueChartSource CreateSource(string id, string envRgpackID)
        {
            return new Source(id, envRgpackID);
        }

        public void MoveNext()
        {
            var nextCmn = Asset.GetNextCmnFrom(CurrentCmn);
            if (nextCmn == null) return; // �I�[�ɒB���Ă���ꍇ�͉������Ȃ�

            CurrentCmn = nextCmn.Cmn; // nextFrom() ���Ŏg�p���邽�߃R�����C�x���g���s�O�ɐݒ肷��
            nextCmn.Invoke();
        }

        [Objforming.Formable]
        private class Source : RgpackReference<ChartPadAsset>, IRogueChartSource
        {
            public CmnReference CurrentCmn { get; set; }

            public Source(string id, string envRgpackID)
                : base(id, envRgpackID)
            {
            }

            public IRogueChart CreateChart()
            {
                return new ChartPadReference(FullID, RgpackID);
            }

            public bool Equals(IRogueChartSource other)
            {
                return other is Source reference && reference.FullID == FullID;
            }

            public override bool Equals(object obj)
            {
                return obj is Source reference && reference.FullID == FullID;
            }

            public override int GetHashCode()
            {
                return FullID.GetHashCode();
            }
        }
    }
}
