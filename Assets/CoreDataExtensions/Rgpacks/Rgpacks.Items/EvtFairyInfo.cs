using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class EvtFairyInfo
    {
        public string RelatedChart { get; set; }
        private readonly List<Point> _points = new();

        public Spanning<Point> Points => _points;

        private EvtFairyInfo() { }

        public Point AddPoint()
        {
            var point = new Point();
            point.ChartCmn = null;
            point.IfCmn = new PropertiedCmnData();
            point.Sprite = null;
            point.Cmn = new PropertiedCmnData();
            _points.Add(point);
            return point;
        }

        //public bool AddPointClones(IEnumerable<Point> points) => _points.AddRange(points.Select(x => x.Clone()));
        public bool RemovePoint(Point point) => _points.Remove(point);
        public void ClearPoints() => _points.Clear();

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
            public string ChartCmn { get; set; }
            public PropertiedCmnData IfCmn { get; set; }
            public string Sprite { get; set; }
            public Category Category { get; set; }
            public PropertiedCmnData Cmn { get; set; }
            public Vector2Int Position { get; set; }
        }

        [Objforming.Formable]
        public enum Category
        {
            ApplyTool,
            Trap
        }
    }
}
