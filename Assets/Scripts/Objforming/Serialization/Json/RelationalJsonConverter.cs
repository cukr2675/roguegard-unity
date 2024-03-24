using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Objforming.Serialization.Json
{
    public abstract class RelationalJsonConverter : JsonConverter, IRelationalComponent
    {
        [JsonIgnore] public abstract Type InstanceType { get; }

        [JsonIgnore] public abstract IReadOnlyList<Type> FieldTypes { get; }

        [JsonIgnore] public virtual IReadOnlyDictionary<string, object> References => empty;
        private static readonly IReadOnlyDictionary<string, object> empty = new Dictionary<string, object>();

        [JsonIgnore] public override bool CanRead => base.CanRead;
        [JsonIgnore] public override bool CanWrite => base.CanWrite;

        public override bool CanConvert(Type objectType)
        {
            return objectType == InstanceType;
        }

        public virtual bool Overrides(IRelationalComponent other) => other.InstanceType == InstanceType;
    }
}
