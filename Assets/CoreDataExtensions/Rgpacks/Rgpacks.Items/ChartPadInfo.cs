using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class ChartPadInfo
    {
        private readonly List<PropertiedCmnData> _cmns = new();
        public Spanning<PropertiedCmnData> Cmns => _cmns;

        private ChartPadInfo() { }

        public PropertiedCmnData AddCmn()
        {
            var newCmn = new PropertiedCmnData();
            _cmns.Add(newCmn);
            return newCmn;
        }

        //public bool AddCmnClones(IEnumerable<PropertiedCmnData> cmns) => _cmns.AddRange(cmns.Select(x => x.Clone()));
        public bool RemoveCmn(PropertiedCmnData cmn) => _cmns.Remove(cmn);
        public void ClearCmns() => _cmns.Clear();

        public static ChartPadInfo Get(RogueObj obj)
        {
            if (obj.TryGet<Info>(out var info))
            {
                return info.chart;
            }
            else
            {
                return null;
            }
        }

        public static void SetTo(RogueObj obj)
        {
            if (!obj.TryGet<Info>(out var info))
            {
                info = new Info();
                obj.SetInfo(info);
            }

            info.chart = new ChartPadInfo();
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public ChartPadInfo chart;

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
    }
}
