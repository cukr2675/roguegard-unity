using Newtonsoft.Json;
using System;

namespace Objforming.Serialization.Json
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

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var member = Former.Members[0];
            var memberValue = member.GetValue(value);

            if (memberValue == null)
            {
                ObjformingLogger.LogWarning($"ラッパークラスインスタンス {value} の値が null です。このインスタンスは逆シリアル化時に null となります。");
            }

            serializer.Serialize(writer, memberValue, member.FieldType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            var member = Former.Members[0];
            var memberValue = serializer.Deserialize(reader, member.FieldType);

            var value = existingValue ?? Former.CreateInstance();
            member.SetValue(value, memberValue);
            return value;
        }
    }
}
