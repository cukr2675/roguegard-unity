using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Text.Json;

namespace ObjectFormer.Serialization.TextJson
{
    public class FormerJsonConverter : RelationalJsonConverter
    {
        public override Type InstanceType => Former.InstanceType;

        public override IReadOnlyList<Type> FieldTypes { get; }

        protected Former Former { get; }

        public FormerJsonConverter(Former former)
        {
            Former = former;
            FieldTypes = former.Members.Select(x => x.FieldType).ToArray();
        }

        public static FormerJsonConverter Create(Type type, bool force = false)
        {
            var members = FormerMember.Generate(type, force);
            var former = new Former(type, members);
            return new FormerJsonConverter(former);
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            if (JsonConverterUtility.WriteReferenceOrIDAndType(writer, value, options))
            {
                for (int i = 0; i < Former.Members.Count; i++)
                {
                    var member = Former.Members[i];
                    var memberValue = member.GetValue(value);
                    writer.WritePropertyName(member.CamelName);
                    JsonSerializer.Serialize(writer, memberValue, member.FieldType, options);
                }
            }
            writer.WriteEndObject();
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            if (JsonConverterUtility.ReadTryResolveReference(ref reader, options, out var referencedValue, out var id, out _)) return referencedValue;

            var value = Former.CreateInstance();
            JsonConverterUtility.AddReference(id, value, options);
            while (true)
            {
                if (reader.TokenType == JsonTokenType.EndObject) break;

                var propertyName = reader.GetString();

                reader.Read();
                if (!Former.TryGetMemberByCamel(propertyName, out var member)) continue;

                var memberValue = JsonSerializer.Deserialize(ref reader, member.FieldType, options);
                member.SetValue(value, memberValue);

                reader.Read();
            }
            return value;
        }
    }
}
