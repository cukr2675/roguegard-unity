using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using ObjectFormer;
using ObjectFormer.Serialization.Json;

namespace Roguegard.RogueObjectFormer.Json
{
    public class RoguegardAssetTableJsonConverter : RelationalJsonConverter
    {
        [JsonProperty("space")] private readonly string space;

        public override System.Type InstanceType => typeof(object);

        public override IReadOnlyList<System.Type> FieldTypes => empty;
        private static readonly IReadOnlyList<System.Type> empty = new System.Type[0];

        private IReadOnlyDictionary<string, object> _references;
        public override IReadOnlyDictionary<string, object> References => _references ??= RoguegardSettings.GetAssetTable(space);

        public override bool CanWrite => false;
        public override bool CanRead => false;

        public RoguegardAssetTableJsonConverter(string space)
        {
            this.space = space;
        }

        public override bool Overrides(IRelationalComponent other)
        {
            return false;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new System.NotSupportedException();
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new System.NotSupportedException();
        }
    }
}
