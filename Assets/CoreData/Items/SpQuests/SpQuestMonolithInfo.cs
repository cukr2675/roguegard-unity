using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class SpQuestMonolithInfo
    {
        public string ID { get; }

        public RgpackReference MainChart { get; set; }

        public void LoadFullID(string rgpackID)
        {
            MainChart.LoadFullID(rgpackID);
        }

        public static SpQuestMonolithInfo Get(RogueObj monolith)
        {
            if (monolith.TryGet<Info>(out var info))
            {
                return info.info;
            }
            else
            {
                return null;
            }
        }

        public static void SetTo(RogueObj monolith)
        {
            if (!monolith.TryGet<Info>(out var info))
            {
                info = new Info();
                monolith.SetInfo(info);
            }

            // ã‘‚«•s‰Â
            if (info.info != null) throw new RogueException();

            info.info = new SpQuestMonolithInfo();
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public SpQuestMonolithInfo info;

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo other)
            {
                throw new System.NotImplementedException();
            }

            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
            {
                throw new System.NotImplementedException();
            }

            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
