using System.Collections.Generic;

namespace Roguegard
{
    [Objforming.Formable]
    public class InfoSetReferenceInfo
    {
        private readonly List<IMainInfoSet> infoSets;

        public int Count => infoSets.Count;

        private InfoSetReferenceInfo()
        {
            infoSets = new List<IMainInfoSet>();
        }

        private InfoSetReferenceInfo(IEnumerable<IMainInfoSet> infoSets)
        {
            this.infoSets = new List<IMainInfoSet>(infoSets);
        }

        public IMainInfoSet Get(int index)
        {
            return infoSets[index];
        }

        public static InfoSetReferenceInfo Get(RogueObj obj)
        {
            if (obj.TryGet<Info>(out var info))
            {
                return info.info;
            }
            return null;
        }

        public static void SetTo(RogueObj obj, IEnumerable<IMainInfoSet> infoSets)
        {
            if (!obj.TryGet<Info>(out _))
            {
                obj.SetInfo(new Info
                {
                    info = new InfoSetReferenceInfo(infoSets)
                });
            }
            else
            {
                throw new RogueException("上書き不可");
            }
        }

        public static void SetTo(RogueObj obj, params IMainInfoSet[] infoSets)
        {
            SetTo(obj, (IEnumerable<IMainInfoSet>)infoSets);
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public InfoSetReferenceInfo info;

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo coming)
            {
                if (coming is not Info comingInfo) return false;
                if (info.Count != comingInfo.info.Count) return false;
                for (int i = 0; i < info.Count; i++)
                {
                    if (!info.Get(i).Equals(comingInfo.info.Get(i))) return false;
                }
                return true;
            }

            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
            {
                return new Info() { info = new InfoSetReferenceInfo(info.infoSets) };
            }

            public IRogueObjInfo ReplaceObj(RogueObj obj, RogueObj clonedObj)
            {
                return this;
            }
        }
    }
}
