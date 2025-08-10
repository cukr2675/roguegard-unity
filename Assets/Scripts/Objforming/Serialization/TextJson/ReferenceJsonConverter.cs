using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Objforming.Serialization.TextJson
{
    /// <summary>
    /// <see cref="ReferableAttribute"/> が付与されている型のシリアル化と、逆シリアル時の <see cref="ReferenceResolver"/> を準備するクラス
    /// </summary>
    internal class ReferenceJsonConverter : JsonConverter<object>
    {
        private readonly Dictionary<Type, Dictionary<string, object>> referableInstanceTable = new();

        /// <summary>
        /// <see cref="ReferableAttribute"/> が付与されている型のキーとインスタンスを登録する
        /// </summary>
        public void RegisterReferableInstances(IReadOnlyDictionary<string, object> referableInstanceTable)
        {
            // 引数に Referable 属性が付与されていない型のインスタンスが含まれる場合、例外を投げる
            var notReferableTypes = referableInstanceTable
                .Select(x => x.Value.GetType())
                .Distinct()
                .Where(x => !x.IsDefined(typeof(ReferableAttribute)))
                .ToArray();
            if (notReferableTypes.Length >= 1)
            {
                foreach (var notReferableType in notReferableTypes)
                {
                    ObjformingLogger.LogError(string.Join(", ", referableInstanceTable.Where(x => x.Value.GetType() == notReferableType)));
                }
                throw new ArgumentException(string.Join<Type>(", ", notReferableTypes) + " は Referable ではありません。", nameof(referableInstanceTable));
            }

            // キーとインスタンスを登録する
            foreach (var pair in referableInstanceTable)
            {
                var type = pair.Value.GetType();
                if (!this.referableInstanceTable.TryGetValue(type, out var typedReferableInstanceTable))
                {
                    typedReferableInstanceTable = new Dictionary<string, object>();
                    this.referableInstanceTable.Add(type, typedReferableInstanceTable);
                }

                typedReferableInstanceTable.Add(pair.Key, pair.Value);
            }
        }

        public void SetReferences(JsonSerializerOptions options)
        {
            if (referableInstanceTable.Count == 0) return;

            // 最初のメンバーが登録済みであれば、すべて登録されていると判断して何もしない
            var firstId = referableInstanceTable.First().Value.First().Key;
            var referenceResolver = options.ReferenceHandler.CreateResolver();
            if (referenceResolver.ResolveReference(firstId) != null) return;

            // デシリアライズ時はこのクラスではなく ReferenceResolver で解決するため、前もって登録しておく
            foreach (var typedReferableInstanceTable in referableInstanceTable.Values)
            {
                foreach (var pair in typedReferableInstanceTable)
                {
                    referenceResolver.AddReference(pair.Key, pair.Value);
                }
            }
        }

        public override bool CanConvert(Type typeToConvert)
        {
            var result = referableInstanceTable.Keys.Contains(typeToConvert);
            if (result) return true;

            // このコンバーターは必ず最後に実行されるので、ここで例外判定する
            if (typeToConvert.IsDefined(typeof(ReferableAttribute))) throw new InvalidOperationException(
                $"{typeToConvert} には {nameof(ReferableAttribute)} が設定されていますが、該当するコンバーターが見つかりません。");
            if (typeToConvert.IsDefined(typeof(FormableAttribute))) throw new InvalidOperationException(
                $"{typeToConvert} には {nameof(FormableAttribute)} が設定されていますが、該当するコンバーターが見つかりません。");
            return false;
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            var type = value.GetType();
            if (referableInstanceTable.TryGetValue(type, out var typedReferableInstanceTable))
            {
                foreach (var pair in typedReferableInstanceTable)
                {
                    if (pair.Value != value) continue;

                    writer.WriteString("$ref", pair.Key);
                    writer.WriteEndObject();
                    return;
                }
            }
            throw new InvalidOperationException($"{value} は登録されていません。");
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }
    }
}
