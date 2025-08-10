using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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
            if (ReferenceResolverUtility.WriteReferenceOrIDAndType(writer, value, serializer))
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
            if (ReferenceResolverUtility.ReadTryResolveReference(reader, serializer, out var resolvedValue, out var id, out _)) return resolvedValue;

            var value = existingValue ?? Former.CreateInstance();
            ReferenceResolverUtility.AddReference(id, value, serializer);
            while (true)
            {
                if (reader.TokenType == JsonToken.EndObject) break;

                var propertyName = (string)reader.Value;
                reader.Read(); // キーを飛ばす

                if (!Former.TryGetMemberByCamel(propertyName, out var member))
                {
                    // 無効なプロパティ名の場合
                    reader.Skip(); // 配列またはオブジェクトを飛ばす
                    reader.Read(); // 数値、文字列、boolean、nullまたは配列とオブジェクトの終端を飛ばす
                    continue;
                }

                var memberValue = serializer.Deserialize(reader, member.FieldType);
                member.SetValue(value, memberValue);
                reader.Read(); // メンバデシリアライズ後の終端を飛ばす
            }
            return value;
        }
    }
}
