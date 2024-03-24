using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Objforming.Unity.RuntimeInspector
{
    public abstract class RelationalForm : IRelationalComponent
    {
        public abstract System.Type InstanceType { get; }

        public abstract IReadOnlyList<System.Type> FieldTypes { get; }

        public virtual IReadOnlyDictionary<string, object> References => empty;
        private static readonly IReadOnlyDictionary<string, object> empty = new Dictionary<string, object>();

        public virtual bool CanConvert(System.Type objectType)
        {
            return objectType == InstanceType;
        }

        public abstract void SetPageTo(FormInspector inspector, object value);

        public abstract void AppendElementTo(FormInspector inspector, string key, ElementValueGetter getter, ElementValueSetter setter);

        public virtual bool Overrides(IRelationalComponent other) => other.InstanceType == InstanceType;
    }
}
