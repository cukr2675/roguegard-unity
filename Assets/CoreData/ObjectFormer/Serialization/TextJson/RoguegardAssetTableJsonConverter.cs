using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text.Json;
using ObjectFormer;
using ObjectFormer.Serialization.TextJson;

namespace Roguegard.RogueObjectFormer.TextJson
{
    public class RoguegardAssetTableJsonConverter : RelationalJsonConverter
    {
        private readonly string space;

        public override System.Type InstanceType => typeof(object);

        public override IReadOnlyList<System.Type> FieldTypes => empty;
        private static readonly IReadOnlyList<System.Type> empty = new System.Type[0];

        private IReadOnlyDictionary<string, object> _references;
        public override IReadOnlyDictionary<string, object> References => _references ??= RoguegardSettings.GetAssetTable(space);

        public RoguegardAssetTableJsonConverter(string space)
        {
            this.space = space;
        }

        public override bool Overrides(IRelationalComponent other)
        {
            return false;
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new System.NotSupportedException();
        }

        public override object Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
        {
            throw new System.NotSupportedException();
        }
    }
}
