using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    public class EvtFairyAsset : IEvtAsset
    {
        private readonly string fullId;

        private readonly IRogueChartSource relatedChartSource;
        private readonly List<Page> pages;

        public EvtFairyAsset(EvtFairyInfo info, string envRgpackId, string fullId)
        {
            relatedChartSource = ChartPadReference.CreateSource(info.RelatedChart, envRgpackId);
            pages = new List<Page>();
            foreach (var infoPage in info.Pages)
            {
                pages.Add(new Page
                {
                    ChartCmn = new CmnReference(infoPage.ChartCmn, envRgpackId),
                    IfCmn = infoPage.IfCmn.ToReference(envRgpackId),
                    Sprite = new RogueObjSpriteReference(infoPage.Sprite, envRgpackId),
                    Category = infoPage.Category,
                    Cmn = infoPage.Cmn.ToReference(envRgpackId),
                    Position = infoPage.Position
                });
            }

            this.fullId = fullId;
        }

        public EvtFairyReference GetInfoSet()
        {
            var worldInfo = RogueWorldInfo.GetByCharacter(RogueDevice.Primary.Player);
            if (worldInfo.ChartState.TryGet<ChartPadReference>(relatedChartSource, out _))
            {
                foreach (var page in pages)
                {
                    //if (page.ChartCmn != currentCmn) continue;

                    //var ifCmn = page.IfCmn?.GetData<IScriptingCmn>();
                    //ifCmn?.Invoke();

                    return page.GetInfoSet(fullId, "");
                }
            }
            return pages[0].GetInfoSet(fullId, "");
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

            public EvtFairyReference GetInfoSet(string id, string envRgpackId)
            {
                infoSet ??= new EvtFairyReference(id, envRgpackId, this);
                return infoSet;
            }
        }
    }
}
