using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                var info = new Info();
                info.info = new InfoSetReferenceInfo(infoSets);
                obj.SetInfo(info);
            }
            else
            {
                throw new RogueException("è„èëÇ´ïsâ¬");
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

            public bool CanStack(IRogueObjInfo other)
            {
                if (!(other is Info otherInfo)) return false;
                if (info.Count != otherInfo.info.Count) return false;
                for (int i = 0; i < info.Count; i++)
                {
                    if (!info.Get(i).Equals(otherInfo.info.Get(i))) return false;
                }
                return true;
            }

            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
            {
                return new Info() { info = new InfoSetReferenceInfo(info.infoSets) };
            }

            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj)
            {
                return this;
            }
        }
    }
}
