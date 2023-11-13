using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace ObjectFormer.Serialization.Json
{
    internal class ReferenceJsonConverter : JsonConverter
    {
        private readonly Dictionary<Type, Dictionary<string, object>> values = new Dictionary<Type, Dictionary<string, object>>();

        public override bool CanRead => false;

        public void Add(IReadOnlyDictionary<string, object> table)
        {
            var notReferableTypes = table
                .Select(x => x.Value.GetType())
                .Distinct()
                .Where(x => !x.IsDefined(typeof(ReferableAttribute)))
                .ToArray();
            if (notReferableTypes.Length >= 1)
            {
                foreach (var notReferableType in notReferableTypes)
                {
                    ObjectFormerLogger.LogError(string.Join(", ", table.Where(x => x.Value.GetType() == notReferableType)));
                }
                throw new Exception(string.Join<Type>(", ", notReferableTypes) + " は Referable ではありません。");
            }

            foreach (var pair in table)
            {
                var type = pair.Value.GetType();
                if (!values.TryGetValue(type, out var typedValues))
                {
                    typedValues = new Dictionary<string, object>();
                    values.Add(type, typedValues);
                }

                typedValues.Add(pair.Key, pair.Value);
            }
        }

        public void SetReferences(JsonSerializer serializer)
        {
            if (values.Count == 0) return;

            // 最初のメンバーが登録済みであれば、すべて登録されていると判断して何もしない
            var firstValue = values.First().Value.First().Value;
            if (serializer.ReferenceResolver.IsReferenced(serializer, firstValue)) return;

            foreach (var table in values.Values)
            {
                foreach (var pair in table)
                {
                    serializer.ReferenceResolver.AddReference(serializer, pair.Key, pair.Value);
                }
            }
        }

        public override bool CanConvert(Type objectType)
        {
            var result = values.Keys.Contains(objectType);
            if (result) return true;

            // シリアル化するときこのコンバーターは必ず最後に実行されるので、ここで例外判定する
            if (objectType.IsDefined(typeof(ReferableAttribute))) throw new Exception(
                $"{objectType} には {nameof(ReferableAttribute)} が設定されていますが、該当するコンバーターが見つかりません。");
            if (objectType.IsDefined(typeof(FormableAttribute))) throw new Exception(
                $"{objectType} には {nameof(FormableAttribute)} が設定されていますが、該当するコンバーターが見つかりません。");
            return false;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            var type = value.GetType();
            if (values.TryGetValue(type, out var table))
            {
                foreach (var pair in table)
                {
                    if (pair.Value != value) continue;

                    writer.WritePropertyName("$ref");
                    writer.WriteValue(pair.Key);
                    writer.WriteEndObject();
                    return;
                }
            }
            throw new Exception($"{value} は登録されていません。");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}
