using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class EvtFairyInfo
    {
        public string RelatedChart { get; set; }

        private readonly List<Page> _pages = new();
        public Spanning<Page> Pages => Spanning.Get(_pages);

        private EvtFairyInfo() { }

        public Page AddPage()
        {
            var page = new Page
            {
                ChartCmn = null,
                IfCmn = new PropertiedCmnData(),
                Sprite = null,
                Cmn = new PropertiedCmnData()
            };
            _pages.Add(page);
            return page;
        }

        //public bool AddPageClones(IEnumerable<Page> pages) => _pages.AddRange(pages.Select(x => x.Clone()));
        public bool RemovePage(Page page) => _pages.Remove(page);
        public void ClearPages() => _pages.Clear();

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
        /// 上書き不可
        /// </summary>
        public static void SetTo(RogueObj obj)
        {
            if (!obj.TryGet<Info>(out var info))
            {
                info = new Info();
                obj.SetInfo(info);
            }

            // 上書き不可
            if (info.info != null) throw new System.InvalidOperationException();

            info.info = new EvtFairyInfo();
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public EvtFairyInfo info;

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo coming) => false;
            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueObjInfo ReplaceObj(RogueObj obj, RogueObj clonedObj) => this;
        }

        [Objforming.Formable]
        public class Page
        {
            public string ChartCmn { get; set; }
            public PropertiedCmnData IfCmn { get; set; }
            public string Sprite { get; set; }
            public EvtFairyCategory Category { get; set; }
            public PropertiedCmnData Cmn { get; set; }
            public Vector2Int Position { get; set; }
        }
    }
}
