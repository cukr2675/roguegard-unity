using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Objforming.Serialization.TextJson
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

        public static FormerJsonConverter Create(Type type, bool force = false, bool includeObjectMember = false)
        {
            var members = FormerMember.Generate(type, force, includeObjectMember);
            var former = new Former(type, members);
            return new FormerJsonConverter(former);
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            if (ReferenceResolverUtility.WriteReferenceOrIDAndType(writer, value, options))
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
            if (ReferenceResolverUtility.ReadTryResolveReference(ref reader, options, out var resolvedValue, out var id, out _)) return resolvedValue;

            var value = Former.CreateInstance();
            ReferenceResolverUtility.AddReference(id, value, options);
            while (true)
            {
                if (reader.TokenType == JsonTokenType.EndObject) break;

                var propertyName = reader.GetString();
                reader.Read(); // キーを飛ばす

                if (!Former.TryGetMemberByCamel(propertyName, out var member))
                {
                    // 無効なプロパティ名の場合
                    reader.Skip(); // 配列またはオブジェクトを飛ばす
                    reader.Read(); // 数値、文字列、boolean、nullまたは配列とオブジェクトの終端を飛ばす
                    continue;
                }

                var memberValue = JsonSerializer.Deserialize(ref reader, member.FieldType, options);
                member.SetValue(value, memberValue);
                reader.Read(); // メンバデシリアライズ後の終端を飛ばす
            }
            return value;
        }
    }
}
