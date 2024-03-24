using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Objforming.Unity.RuntimeInspector
{
    public class FormerForm : LinkRelationalForm
    {
        public override System.Type InstanceType => Former.InstanceType;

        public override IReadOnlyList<System.Type> FieldTypes { get; }

        protected Former Former { get; }

        public FormerForm(Former former, LinkElement linkElementPrefab)
            : base(linkElementPrefab)
        {
            Former = former;
            FieldTypes = former.Members.Select(x => x.FieldType).ToArray();
        }

        public static FormerForm Create(System.Type type, LinkElement linkElementPrefab, bool force = false, bool includeObjectMember = false)
        {
            var members = FormerMember.Generate(type, force, includeObjectMember);
            var former = new Former(type, members);
            return new FormerForm(former, linkElementPrefab);
        }

        public override void SetPageTo(FormInspector inspector, object value)
        {
            for (int i = 0; i < Former.Members.Count; i++)
            {
                var member = Former.Members[i];
                var type = member.FieldType;
                inspector.AppendElement(member.Name, member.FieldType, () => member.GetValue(value), x => member.SetValue(value, x));
            }
        }
    }
}
