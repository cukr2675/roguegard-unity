using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using Newtonsoft.Json;

namespace Objforming.Serialization.Json
{
    public class FormerJsonConverter : RelationalJsonConverter
    {
        public override Type InstanceType => Former.InstanceType;

        public override IReadOnlyList<Type> FieldTypes { get; }

        [JsonProperty("former")] protected Former Former { get; }

        public FormerJsonConverter(Former former)
        {
            Former = former;
            FieldTypes = former.Members.Select(x => x.FieldType).ToArray();
        }

        public static FormerJsonConverter Create(Type type, bool force = false, bool includeObjectMember = false)
        {
            var members = FormerMember.Generate(type, force, includeObjectMember);
            var former = new Former(type, members);
            return new FormerJsonConverter(former);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            if (JsonConverterUtility.WriteReferenceOrIDAndType(writer, value, serializer))
            {
                for (int i = 0; i < Former.Members.Count; i++)
                {
                    var member = Former.Members[i];
                    var memberValue = member.GetValue(value);
                    writer.WritePropertyName(member.CamelName);
                    serializer.Serialize(writer, memberValue, member.FieldType);
                }
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            if (JsonConverterUtility.ReadTryResolveReference(reader, serializer, out var referencedValue, out var id, out _)) return referencedValue;

            var value = existingValue ?? Former.CreateInstance();
            JsonConverterUtility.AddReference(id, value, serializer);
            while (true)
            {
                if (reader.TokenType == JsonToken.EndObject) break;

                var propertyName = (string)reader.Value;

                reader.Read(); // ƒL[‚ð”ò‚Î‚·
                if (!Former.TryGetMemberByCamel(propertyName, out var member))
                {
                    reader.Read(); // ’l‚ð”ò‚Î‚·
                    continue;
                }

                var memberValue = serializer.Deserialize(reader, member.FieldType);
                member.SetValue(value, memberValue);

                reader.Read();
            }
            return value;
        }
    }
}
