using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ObjectFormer.Serialization.Json
{
    /// <summary>
    /// 指定の <see cref="Type"/> を Json 化しようとしたとき例外を投げるコンバーター
    /// </summary>
    public class ThrowExceptionJsonConverter : RelationalJsonConverter
    {
        [JsonProperty("instanceType")] public override Type InstanceType { get; }

        public override IReadOnlyList<Type> FieldTypes => empty;
        private static readonly IReadOnlyList<Type> empty = new Type[0];

        public ThrowExceptionJsonConverter(Type type)
        {
            InstanceType = type;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new Exception($"{nameof(ThrowExceptionJsonConverter)} により {value} で例外を投げました。");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new Exception($"{nameof(ThrowExceptionJsonConverter)} により {objectType} ({existingValue}) で例外を投げました。");
        }
    }
}
