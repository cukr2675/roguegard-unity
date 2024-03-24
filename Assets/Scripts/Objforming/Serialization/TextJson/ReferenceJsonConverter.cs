using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Objforming.Serialization.TextJson
{
    internal class ReferenceJsonConverter : JsonConverter<object>
    {
        private readonly Dictionary<Type, Dictionary<string, object>> values = new Dictionary<Type, Dictionary<string, object>>();

        public void Add(IReadOnlyDictionary<string, object> table)
        {
            var notReferableTypes = table
                .Select(x => x.Value.GetType())
                .Distinct()
                .Where(x => !x.IsDefined(typeof(ReferableAttribute)))
                .ToArray();
            if (notReferableTypes.Length >= 1) throw new Exception(string.Join<Type>(", ", notReferableTypes) + " は Referable ではありません。");

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

        public void SetReferences(JsonSerializerOptions options)
        {
            if (values.Count == 0) return;

            // 最初のメンバーが登録済みであれば、すべて登録されていると判断して何もしない
            var firstID = values.First().Value.First().Key;
            var referenceResolver = options.ReferenceHandler.CreateResolver();
            if (referenceResolver.ResolveReference(firstID) != null) return;

            foreach (var table in values.Values)
            {
                foreach (var pair in table)
                {
                    referenceResolver.AddReference(pair.Key, pair.Value);
                }
            }
        }

        public override bool CanConvert(Type typeToConvert)
        {
            var result = values.Keys.Contains(typeToConvert);
            if (result) return true;

            // このコンバーターは必ず最後に実行されるので、ここで例外判定する
            if (typeToConvert.IsDefined(typeof(ReferableAttribute))) throw new Exception(
                $"{typeToConvert} には {nameof(ReferableAttribute)} が設定されていますが、該当するコンバーターが見つかりません。");
            if (typeToConvert.IsDefined(typeof(FormableAttribute))) throw new Exception(
                $"{typeToConvert} には {nameof(FormableAttribute)} が設定されていますが、該当するコンバーターが見つかりません。");
            return false;
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            var type = value.GetType();
            if (values.TryGetValue(type, out var table))
            {
                foreach (var pair in table)
                {
                    if (pair.Value != value) continue;

                    writer.WriteString("$ref", pair.Key);
                    writer.WriteEndObject();
                    return;
                }
            }
            throw new Exception($"{value} は登録されていません。");
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }
    }
}
