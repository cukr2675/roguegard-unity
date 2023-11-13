using System;
using System.Collections;
using System.Collections.Generic;

using System.Text.Json;
using System.Text.Json.Nodes;

namespace ObjectFormer.Serialization.TextJson
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

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            if (JsonConverterUtility.WriteReferenceOrIDAndType(writer, value, options))
            {
                var array = (T[])value;
                writer.WritePropertyName("$length");
                JsonSerializer.Serialize(writer, array.Length, options);

                writer.WritePropertyName("$values");
                writer.WriteStartArray();
                foreach (var item in array)
                {
                    JsonSerializer.Serialize(writer, item, typeof(T), options);
                }
                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;

            var jObj = JsonNode.Parse(ref reader).AsObject();
            if (JsonConverterUtility.TryResolveReference(jObj["$ref"]?.ToString(), options, out var referencedValue)) return referencedValue;

            var length = (int)jObj["$length"];
            var array = new T[length];
            JsonConverterUtility.AddReference(jObj["$id"]?.ToString(), array, options);

            var jArray = jObj["$values"].AsArray();
            var i = 0;
            foreach (var item in jArray)
            {
                var value = JsonSerializer.Deserialize(item, typeof(T), options);
                array.SetValue(value, i);
                i++;
            }
            return array;
        }
    }
}
