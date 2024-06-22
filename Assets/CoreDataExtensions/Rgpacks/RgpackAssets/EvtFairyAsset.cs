using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    public class EvtFairyAsset : IEvtAsset
    {
        private readonly string fullID;

        private readonly IRogueChartSource relatedChartSource;
        private readonly List<Point> points;

        public EvtFairyAsset(EvtFairyInfo info, string envRgpackID, string fullID)
        {
            relatedChartSource = ChartPadReference.CreateSource(info.RelatedChart, envRgpackID);
            points = new List<Point>();
            for (int i = 0; i < info.Points.Count; i++)
            {
                var infoPoint = info.Points[i];
                var newPoint = new Point();
                newPoint.ChartCmn = new CmnReference(infoPoint.ChartCmn, envRgpackID);
                newPoint.IfCmn = infoPoint.IfCmn.ToReference(envRgpackID);
                newPoint.Sprite = new RogueObjSpriteReference(infoPoint.Sprite, envRgpackID);
                newPoint.Category = infoPoint.Category;
                newPoint.Cmn = infoPoint.Cmn.ToReference(envRgpackID);
                newPoint.Position = infoPoint.Position;
                newPoint.Position = new Vector2Int(3, 3);
                points.Add(newPoint);
            }

            this.fullID = fullID;
        }

        public EvtFairyReference GetInfoSet()
        {
            var worldInfo = RogueWorldInfo.GetByCharacter(RogueDevice.Primary.Player);
            if (worldInfo.ChartState.TryGet<ChartPadReference>(relatedChartSource, out var chart))
            {
                foreach (var point in points)
                {
                    //if (point.ChartCmn != currentCmn) continue;

                    //var ifCmn = point.IfCmn?.GetData<IScriptingCmn>();
                    //ifCmn?.Invoke();

                    return point.GetInfoSet(fullID, "");
                }
            }
            return points[0].GetInfoSet(fullID, "");
        }

        public class Point
        {
            public CmnReference ChartCmn { get; set; }
            public PropertiedCmnReference IfCmn { get; set; }
            public RogueObjSpriteReference Sprite { get; set; }
            public EvtFairyInfo.Category Category { get; set; }
            public PropertiedCmnReference Cmn { get; set; }
            public Vector2Int Position { get; set; }

            private EvtFairyReference infoSet;

            public EvtFairyReference GetInfoSet(string id, string envRgpackID)
            {
                if (infoSet == null)
                {
                    infoSet = new EvtFairyReference(id, envRgpackID, this);
                }
                return infoSet;
            }
        }
    }
}
