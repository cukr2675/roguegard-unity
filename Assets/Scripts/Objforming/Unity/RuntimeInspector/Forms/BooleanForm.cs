using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace Objforming.Unity.RuntimeInspector
{
    public class BooleanForm : RelationalForm
    {
        public override System.Type InstanceType => typeof(bool);

        public override IReadOnlyList<System.Type> FieldTypes => empty;
        private static readonly IReadOnlyList<System.Type> empty = new System.Type[0];

        private readonly ToggleElement toggleElementPrefab;

        public BooleanForm(ToggleElement toggleElementPrefab)
        {
            this.toggleElementPrefab = toggleElementPrefab;
        }

        public override void SetPageTo(FormInspector inspector, object value)
        {
            throw new System.NotImplementedException();
        }

        public override void AppendElementTo(FormInspector inspector, string key, ElementValueGetter getter, ElementValueSetter setter)
        {
            var element = Object.Instantiate(toggleElementPrefab, inspector.Page);
            element.Initialize(inspector, key, getter, setter);
        }
    }
}
