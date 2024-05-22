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
        private readonly List<Point> points = new();

        public Vector2Int Position { get; set; }
        public RgpackReference Appearance { get; set; }
        private RgpackReference[] _opens = new RgpackReference[0];

        public Spanning<RgpackReference> Opens => _opens;

        //private EventFairyInfo() { }

        public void SetRgpackID(string rgpackID)
        {
            Appearance = new RgpackReference(rgpackID, Appearance.AssetID);
            for (int i = 0; i < _opens.Length; i++)
            {
                _opens[i] = new RgpackReference(rgpackID, _opens[i].AssetID);
            }
        }

        public void SetOpens(IEnumerable<RgpackReference> opens)
        {
            _opens = opens.ToArray();
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
            info.info.Appearance = new RgpackReference(null, null);
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
        private class Point
        {
            public RgpackReference ChartCmn { get; }
            public RgpackReference Appearance { get; set; }
            public int Category { get; set; }
            public RgpackReference Cmn { get; set; }
            public Vector2Int Position { get; set; }
        }
    }
}
