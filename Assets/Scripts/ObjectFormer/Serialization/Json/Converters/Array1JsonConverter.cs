using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ObjectFormer.Serialization.Json
{
    public static class Array1JsonConverter
    {
        public static RelationalJsonConverter Create(Type elementType)
        {
            var converterType = typeof(Array1JsonConverter<>).MakeGenericType(elementType);
            return (RelationalJsonConverter)Activator.CreateInstance(converterType);
        }
    }

    /// <summary>
    /// <see cref="Array.Length"/> を含めて Json 化するコンバーター
    /// </summary>
    public class Array1JsonConverter<T> : RelationalJsonConverter
    {
        public override Type InstanceType => typeof(T[]);

        public override IReadOnlyList<Type> FieldTypes { get; } = new[] { typeof(T) };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            if (JsonConverterUtility.WriteReferenceOrIDAndType(writer, value, serializer))
            {
                var array = (T[])value;
                writer.WritePropertyName("$length");
                serializer.Serialize(writer, array.Length);

                writer.WritePropertyName("$values");
                writer.WriteStartArray();
                foreach (var item in array)
                {
                    serializer.Serialize(writer, item, typeof(T));
                }
                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            var jObj = JObject.Load(reader);
            if (JsonConverterUtility.TryResolveReference(jObj["$ref"]?.ToString(), serializer, out var referencedValue)) return referencedValue;

            var existingArray = existingValue as T[];
            var length = (int)jObj["$length"];
            if (existingArray != null && existingArray.Length != length) throw new Exception(
                $"{nameof(Array.Length)} が一致しません。 ({existingArray.Length} != {length})");

            var array = existingArray ?? new T[length];
            JsonConverterUtility.AddReference(jObj["$id"]?.ToString(), array, serializer);

            var jArray = jObj["$values"];
            var i = 0;
            foreach (var item in jArray)
            {
                using var valueReader = item.CreateReader();
                var value = serializer.Deserialize(valueReader, typeof(T));
                array.SetValue(value, i);
                i++;
            }
            return array;
        }
    }
}
