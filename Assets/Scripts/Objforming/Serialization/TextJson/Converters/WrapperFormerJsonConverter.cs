using System;
using System.Text.Json;

namespace Objforming.Serialization.TextJson
{
    public class WrapperFormerJsonConverter : FormerJsonConverter
    {
        public WrapperFormerJsonConverter(Former former)
            : base(former)
        {
            if (FormableAttribute.GetModeOrDefault(former.InstanceType) != FormerMode.Wrapper) throw new ArgumentException(
                $"{Former.InstanceType} は {FormerMode.Wrapper} に設定されていません。");
            if (Former.Members.Count >= 2) throw new ArgumentException(
                $"{Former.InstanceType} は {FormerMode.Wrapper} に設定されていますが、二つ以上のシリアル化対象メンバが存在します。");
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            var member = Former.Members[0];
            var memberValue = member.GetValue(value);
            JsonSerializer.Serialize(writer, memberValue, member.FieldType, options);
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var member = Former.Members[0];
            var memberValue = JsonSerializer.Deserialize(ref reader, member.FieldType, options);

            var value = Former.CreateInstance();
            member.SetValue(value, memberValue);
            return value;
        }
    }
}
