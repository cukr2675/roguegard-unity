using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ObjectFormer;
using ObjectFormer.Unity.RuntimeInspector;

namespace Roguegard.ObjectFormer.RuntimeInspector
{
    public class RogueObjListForm : LinkRelationalForm
    {
        public override System.Type InstanceType => typeof(RogueObjList);

        public override IReadOnlyList<System.Type> FieldTypes => _fieldTypes;
        private static readonly IReadOnlyList<System.Type> _fieldTypes = new[] { typeof(List<RogueObj>) };

        private readonly Former spaceFormer;
        private readonly RogueObjListItemElement itemElementPrefab;

        public RogueObjListForm(Former spaceFormer, RogueObjListItemElement itemElementPrefab, LinkElement linkElementPrefab)
            : base(linkElementPrefab)
        {
            this.spaceFormer = spaceFormer;
            this.itemElementPrefab = itemElementPrefab;
        }

        public static RogueObjListForm Create(RogueObjListItemElement itemElementPrefab, LinkElement linkElementPrefab)
        {
            var type = typeof(RogueSpace);
            var members = FormerMember.Generate(type);
            var former = new Former(type, members);
            return new RogueObjListForm(former, itemElementPrefab, linkElementPrefab);
        }

        public override void SetPageTo(FormInspector inspector, object value)
        {
            var list = (RogueObjList)value;
            for (int i = 0; i < list.Count; i++)
            {
                var itemElement = Object.Instantiate(itemElementPrefab, inspector.Page);
                itemElement.Initialize(inspector, list[i], spaceFormer);
            }
        }
    }
}
