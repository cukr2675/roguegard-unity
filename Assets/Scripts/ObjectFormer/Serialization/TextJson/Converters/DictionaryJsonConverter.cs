using System;
using System.Collections;
using System.Collections.Generic;

using System.Text.Json;
using System.Text.Json.Nodes;

namespace ObjectFormer.Serialization.TextJson
{
    /// <summary>
    /// <see cref="Dictionary{TKey, TValue}.Comparer"/> を含めて Json 化するコンバーター
    /// </summary>
    internal class DictionaryJsonConverter<TKey, TValue> : RelationalJsonConverter
    {
        public override Type InstanceType => typeof(Dictionary<TKey, TValue>);

        public override IReadOnlyList<Type> FieldTypes { get; }
            = new[] { typeof(TKey), typeof(TValue), typeof(IEqualityComparer<TKey>) };

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            if (JsonConverterUtility.WriteReferenceOrIDAndType(writer, value, options))
            {
                var table = (Dictionary<TKey, TValue>)value;
                if (table.Comparer != EqualityComparer<TKey>.Default)
                {
                    writer.WritePropertyName("$comparer");
                    JsonSerializer.Serialize(writer, table.Comparer, options);
                }

                foreach (var pair in table)
                {
                    throw new NotImplementedException();
                    //using var keyWriter = new JTokenWriter();
                    //serializer.Serialize(keyWriter, pair.Key, typeof(TKey));
                    //writer.WritePropertyName(keyWriter.Token.ToString());

                    //serializer.Serialize(writer, pair.Value, typeof(TValue));
                }
            }
            writer.WriteEndObject();
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;

            var jObj = JsonNode.Parse(ref reader).AsObject();
            if (JsonConverterUtility.TryResolveReference(jObj["$ref"]?.ToString(), options, out var referencedValue)) return referencedValue;

            IEqualityComparer<TKey> comparer = null;
            if (jObj.TryGetPropertyValue("$comparer", out var comparerToken))
            {
                comparer = JsonSerializer.Deserialize<IEqualityComparer<TKey>>(comparerToken, options);
            }

            var value = new Dictionary<TKey, TValue>(comparer);
            JsonConverterUtility.AddReference(jObj["$id"]?.ToString(), value, options);

            foreach (var pair in jObj)
            {
                if (pair.Key.StartsWith('$')) continue;

                throw new NotImplementedException();
                //using var keyReader = new JTokenReader(pair.Key);
                //var key = serializer.Deserialize<TKey>(keyReader);
                //using var valueReader = pair.Value.CreateReader();
                //var tableValue = serializer.Deserialize<TValue>(valueReader);
                //value.Add(key, tableValue);
            }
            return value;
        }
    }
}
