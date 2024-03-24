using System;
using System.Collections;
using System.Collections.Generic;

using System.Text.Json.Serialization;

namespace Objforming.Serialization.TextJson
{
    public abstract class RelationalJsonConverter : JsonConverter<object>, IRelationalComponent
    {
        public abstract Type InstanceType { get; }

        public abstract IReadOnlyList<Type> FieldTypes { get; }

        public virtual IReadOnlyDictionary<string, object> References => empty;
        private static readonly IReadOnlyDictionary<string, object> empty = new Dictionary<string, object>();

        public override bool CanConvert(Type objectType)
        {
            return objectType == InstanceType;
        }

        public virtual bool Overrides(IRelationalComponent other) => other.InstanceType == InstanceType;
    }
}
