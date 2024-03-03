using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ObjectFormer.Unity.RuntimeInspector;

namespace Roguegard.RogueObjectFormer.RuntimeInspector
{
    public class RogueObjListForm : LinkRelationalForm
    {
        public override System.Type InstanceType => throw new System.NotImplementedException();

        public override IReadOnlyList<System.Type> FieldTypes => throw new System.NotImplementedException();

        public RogueObjListForm(LinkElement linkElementPrefab)
            : base(linkElementPrefab)
        {
        }

        public override void SetPageTo(FormInspector inspector, object value)
        {
            var list = (RogueObjList)value;
            for (int i = 0; i < list.Count; i++)
            {
                //var element = LinkElement.Get(null, list[i]);
                //element.SetParent(page, false);
            }
        }
    }
}
