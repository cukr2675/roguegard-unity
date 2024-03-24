using System;
using System.Collections;
using System.Collections.Generic;

using System.Text.Json;

namespace Objforming.Serialization.TextJson
{
    /// <summary>
    /// 指定の <see cref="Type"/> を Json 化しようとしたとき例外を投げるコンバーター
    /// </summary>
    public class ThrowExceptionJsonConverter : RelationalJsonConverter
    {
        public override Type InstanceType { get; }

        public override IReadOnlyList<Type> FieldTypes => empty;
        private static readonly IReadOnlyList<Type> empty = new Type[0];

        public ThrowExceptionJsonConverter(Type type)
        {
            InstanceType = type;
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new Exception($"{nameof(ThrowExceptionJsonConverter)} により {value} で例外を投げました。");
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new Exception($"{nameof(ThrowExceptionJsonConverter)} により {typeToConvert} で例外を投げました。");
        }
    }
}
