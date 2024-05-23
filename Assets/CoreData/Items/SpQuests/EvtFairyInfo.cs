using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard
{
    [Objforming.Formable]
    public class EvtFairyInfo
    {
        public string ID { get; set; }
        public RgpackReference RelatedChart { get; set; }
        private readonly List<Point> _points = new();

        public Spanning<Point> Points => _points;

        //private EventFairyInfo() { }

        public void SetRgpackID(RogueObj evtFairy, string rgpackID)
        {
            RelatedChart = new RgpackReference(rgpackID, RelatedChart.AssetID);
            foreach (var point in _points)
            {
                point.ChartCmn = new RgpackReference(rgpackID, point.ChartCmn.AssetID);
                point.IfCmn = new RgpackReference(rgpackID, point.IfCmn.AssetID);
                point.Appearance = new RgpackReference(rgpackID, point.Appearance.AssetID);
                point.Cmn = new RgpackReference(rgpackID, point.Cmn.AssetID);
                point.Position = evtFairy.Position;
            }
        }

        public void AddPoint(RgpackReference chartCmn)
        {
            var point = new Point();
            point.ChartCmn = chartCmn;
            point.IfCmn = new RgpackReference(null, null);
            point.Appearance = new RgpackReference(null, null);
            point.Cmn = new RgpackReference(null, null);
            _points.Add(point);
        }

        public EvtInstanceInfoSet CreateInfoSet()
        {
            var worldInfo = RogueWorldInfo.GetByCharacter(RogueDevice.Primary.Player);
            var currentCmn = worldInfo.ChartState.GetCurrentCmn(RelatedChart);
            foreach (var point in _points)
            {
                //if (point.ChartCmn != currentCmn) continue;

                //var ifCmn = point.IfCmn?.GetData<IScriptingCmn>();
                //ifCmn?.Invoke();

                var appearance = point.Appearance.GetData<KyarakuriFigurineInfo>();
                return new EvtInstanceInfoSet(this, appearance.Main, point.Position);
            }
            throw new RogueException();
        }

        public static EvtFairyInfo Get(RogueObj obj)
        {
            if (obj.TryGet<Info>(out var info))
            {
                return info.info;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// è„èëÇ´ïsâ¬
        /// </summary>
        public static void SetTo(RogueObj obj)
        {
            if (!obj.TryGet<Info>(out var info))
            {
                info = new Info();
                obj.SetInfo(info);
            }

            // è„èëÇ´ïsâ¬
            if (info.info != null) throw new RogueException();

            info.info = new EvtFairyInfo();
            info.info.RelatedChart = new RgpackReference(null, null);
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public EvtFairyInfo info;

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo other)
            {
                return false;
            }

            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
            {
                return null;
            }

            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj)
            {
                return this;
            }
        }

        [Objforming.Formable]
        public class Point
        {
            public RgpackReference ChartCmn { get; set; }
            public RgpackReference IfCmn { get; set; }
            public RgpackReference Appearance { get; set; }
            public int Category { get; set; }
            public RgpackReference Cmn { get; set; }
            public Vector2Int Position { get; set; }
        }
    }
}
