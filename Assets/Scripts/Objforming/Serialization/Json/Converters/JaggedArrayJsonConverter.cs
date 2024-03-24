using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Objforming.Serialization.Json
{
    public static class JaggedArrayJsonConverter
    {
        public static RelationalJsonConverter Create(Type instanceType)
        {
            var converterType = typeof(JaggedArrayJsonConverter<>).MakeGenericType(instanceType);
            return (RelationalJsonConverter)Activator.CreateInstance(converterType);
        }
    }

    public class JaggedArrayJsonConverter<T> : RelationalJsonConverter
    {
        public override Type InstanceType => typeof(T);

        public override IReadOnlyList<Type> FieldTypes => empty;
        private static readonly IReadOnlyList<Type> empty = new Type[0];

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            if (JsonConverterUtility.WriteReferenceOrIDAndType(writer, value, serializer))
            {
                writer.WritePropertyName("elm");
                WriteArray((Array)value, typeof(T).GetElementType());
            }
            writer.WriteEndObject();

            void WriteArray(Array array, Type elementType)
            {
                writer.WriteStartArray();

                // 最初の要素を要素数とする
                writer.WriteValue(array.Length);

                // 以降の要素が実際の要素となる
                if (elementType.IsArray)
                {
                    // 子配列
                    for (int i = 0; i < array.Length; i++)
                    {
                        var item = array.GetValue(i);
                        WriteArray((Array)item, elementType.GetElementType());
                    }
                }
                else
                {
                    // 配列以外の要素
                    for (int i = 0; i < array.Length; i++)
                    {
                        var item = array.GetValue(i);
                        serializer.Serialize(writer, item, elementType);
                    }
                }

                writer.WriteEndArray();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            var jObj = JObject.Load(reader);
            if (JsonConverterUtility.TryResolveReference(jObj["$ref"]?.ToString(), serializer, out var referencedValue)) return referencedValue;

            using var elmReader = jObj["elm"].CreateReader();
            return ReadArray(typeof(T).GetElementType(), true);

            Array ReadArray(Type elementType, bool first)
            {
                elmReader.Read();
                if (elmReader.TokenType != JsonToken.StartArray) throw new JsonException($"{elmReader.TokenType} is not {JsonToken.StartArray}");

                // 最初の要素を要素数とする
                var length = (int)elmReader.ReadAsInt32();
                Array array;
                if (first && existingValue != null)
                {
                    array = (Array)existingValue;
                    if (array.Length != length) throw new Exception("配列の長さが不正です。");
                }
                else
                {
                    array = Array.CreateInstance(elementType, length);
                }

                if (first)
                {
                    JsonConverterUtility.AddReference(jObj["$id"]?.ToString(), array, serializer);
                }

                // 以降の要素が実際の要素となる
                if (elementType.IsArray)
                {
                    // 子配列
                    for (int i = 0; i < array.Length; i++)
                    {
                        var item = ReadArray(elementType.GetElementType(), false);
                        array.SetValue(item, i);
                    }
                }
                else
                {
                    // 配列以外の要素
                    for (int i = 0; i < array.Length; i++)
                    {
                        elmReader.Read();
                        var item = serializer.Deserialize(elmReader, elementType);
                        array.SetValue(item, i);
                    }
                }

                elmReader.Read();
                if (elmReader.TokenType != JsonToken.EndArray) throw new JsonException($"{elmReader.TokenType} is not {JsonToken.EndArray}");

                return array;
            }
        }
    }
}
