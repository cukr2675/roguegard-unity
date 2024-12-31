using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    public class EvtFairyAsset : IEvtAsset
    {
        private readonly string fullID;

        private readonly IRogueChartSource relatedChartSource;
        private readonly List<Page> pages;

        public EvtFairyAsset(EvtFairyInfo info, string envRgpackID, string fullID)
        {
            relatedChartSource = ChartPadReference.CreateSource(info.RelatedChart, envRgpackID);
            pages = new List<Page>();
            for (int i = 0; i < info.Pages.Count; i++)
            {
                var infoPage = info.Pages[i];
                var newPage = new Page();
                newPage.ChartCmn = new CmnReference(infoPage.ChartCmn, envRgpackID);
                newPage.IfCmn = infoPage.IfCmn.ToReference(envRgpackID);
                newPage.Sprite = new RogueObjSpriteReference(infoPage.Sprite, envRgpackID);
                newPage.Category = infoPage.Category;
                newPage.Cmn = infoPage.Cmn.ToReference(envRgpackID);
                newPage.Position = infoPage.Position;
                pages.Add(newPage);
            }

            this.fullID = fullID;
        }

        public EvtFairyReference GetInfoSet()
        {
            var worldInfo = RogueWorldInfo.GetByCharacter(RogueDevice.Primary.Player);
            if (worldInfo.ChartState.TryGet<ChartPadReference>(relatedChartSource, out var chart))
            {
                foreach (var page in pages)
                {
                    //if (page.ChartCmn != currentCmn) continue;

                    //var ifCmn = page.IfCmn?.GetData<IScriptingCmn>();
                    //ifCmn?.Invoke();

                    return page.GetInfoSet(fullID, "");
                }
            }
            return pages[0].GetInfoSet(fullID, "");
        }

        public class Page
        {
            public CmnReference ChartCmn { get; set; }
            public PropertiedCmnReference IfCmn { get; set; }
            public RogueObjSpriteReference Sprite { get; set; }
            public EvtFairyCategory Category { get; set; }
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
