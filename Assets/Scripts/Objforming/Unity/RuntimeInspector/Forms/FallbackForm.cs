using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace Objforming.Unity.RuntimeInspector
{
    public class FallbackForm : RelationalForm
    {
        public override System.Type InstanceType => typeof(string);

        public override IReadOnlyList<System.Type> FieldTypes => empty;
        private static readonly IReadOnlyList<System.Type> empty = new System.Type[0];

        private readonly InputElement inputElementPrefab;

        public FallbackForm(InputElement inputElementPrefab)
        {
            this.inputElementPrefab = inputElementPrefab;
        }

        public override void SetPageTo(FormInspector inspector, object value)
        {
            throw new System.NotImplementedException();
        }

        public override void AppendElementTo(FormInspector inspector, string key, ElementValueGetter getter, ElementValueSetter setter)
        {
            var element = Object.Instantiate(inputElementPrefab, inspector.Page);
            element.Initialize(inspector, key, getter, setter, TMP_InputField.ContentType.Standard, false);
        }
    }
}
