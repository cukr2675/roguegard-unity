using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Objforming.Serialization.Json
{
    public class NonIdNonTypeFormerJsonConverter : FormerJsonConverter
    {
        public NonIdNonTypeFormerJsonConverter(Former former)
            : base(former)
        {
        }

        public static new NonIdNonTypeFormerJsonConverter Create(Type type, bool force = false, bool includeObjectMember = false)
        {
            var members = FormerMember.Generate(type, force, includeObjectMember);
            var former = new Former(type, members);
            return new NonIdNonTypeFormerJsonConverter(former);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            for (int i = 0; i < Former.Members.Count; i++)
            {
                var member = Former.Members[i];
                var memberValue = member.GetValue(value);
                writer.WritePropertyName(member.CamelName);
                serializer.Serialize(writer, memberValue, member.FieldType);
            }
            writer.WriteEndObject();
        }
    }
}
