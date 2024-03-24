using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Objforming.Serialization.Json
{
    /// <summary>
    /// <see cref="Dictionary{TKey, TValue}.Comparer"/> を含めて Json 化するコンバーター
    /// </summary>
    internal class DictionaryJsonConverter<TKey, TValue> : RelationalJsonConverter
    {
        public override Type InstanceType => typeof(Dictionary<TKey, TValue>);

        public override IReadOnlyList<Type> FieldTypes { get; }
            = new[] { typeof(TKey), typeof(TValue), typeof(IEqualityComparer<TKey>) };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            if (JsonConverterUtility.WriteReferenceOrIDAndType(writer, value, serializer))
            {
                var table = (Dictionary<TKey, TValue>)value;
                if (table.Comparer != EqualityComparer<TKey>.Default)
                {
                    writer.WritePropertyName("$comparer");
                    serializer.Serialize(writer, table.Comparer);
                }

                foreach (var pair in table)
                {
                    using var keyWriter = new JTokenWriter();
                    serializer.Serialize(keyWriter, pair.Key, typeof(TKey));
                    writer.WritePropertyName(keyWriter.Token.ToString());

                    serializer.Serialize(writer, pair.Value, typeof(TValue));
                }
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            var jObj = JObject.Load(reader);
            if (JsonConverterUtility.TryResolveReference(jObj["$ref"]?.ToString(), serializer, out var referencedValue)) return referencedValue;

            IEqualityComparer<TKey> comparer = null;
            if (jObj.TryGetValue("$comparer", out var comparerToken))
            {
                using var comparerReader = comparerToken.CreateReader();
                comparer = serializer.Deserialize<IEqualityComparer<TKey>>(comparerReader);
            }

            var existingTable = existingValue as Dictionary<TKey, TValue>;
            if (existingTable != null && existingTable.Comparer != comparer) throw new Exception(
                $"{nameof(Dictionary<TKey, TValue>.Comparer)} が一致しません。 ({existingTable.Comparer} != {comparer})");

            var value = existingTable ?? new Dictionary<TKey, TValue>(comparer);
            JsonConverterUtility.AddReference(jObj["$id"]?.ToString(), value, serializer);

            foreach (var pair in jObj)
            {
                if (pair.Key.StartsWith('$')) continue;

                using var keyReader = new JTokenReader(pair.Key);
                var key = serializer.Deserialize<TKey>(keyReader);
                using var valueReader = pair.Value.CreateReader();
                var tableValue = serializer.Deserialize<TValue>(valueReader);
                value.Add(key, tableValue);
            }
            return value;
        }
    }
}
