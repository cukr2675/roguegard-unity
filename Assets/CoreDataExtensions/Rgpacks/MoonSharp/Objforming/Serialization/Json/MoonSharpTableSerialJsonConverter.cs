using MoonSharp.Interpreter;
using Newtonsoft.Json;
using Objforming.Serialization.Json;
using System.Collections.Generic;

namespace Roguegard.Rgpacks.MoonSharp.Objforming.Serialization.Json
{
    public class MoonSharpTableSerialJsonConverter : RelationalJsonConverter
    {
        public override System.Type InstanceType => typeof(MoonSharpTableSerial);

        public override IReadOnlyList<System.Type> FieldTypes => _fieldTypes;
        private static readonly IReadOnlyList<System.Type> _fieldTypes = new System.Type[1] { typeof(Dictionary<string, object>) };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            if (ReferenceResolverUtility.WriteReferenceOrIDAndType(writer, value, serializer))
            {
                var serial = (MoonSharpTableSerial)value;
                foreach (var pair in serial.Table.Pairs)
                {
                    if (pair.Key.Type != DataType.String) continue;

                    if (pair.Value.Type == DataType.Number)
                    {
                        writer.WritePropertyName(pair.Key.String);
                        serializer.Serialize(writer, pair.Value.Number, typeof(double));
                    }
                }
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            if (ReferenceResolverUtility.ReadTryResolveReference(reader, serializer, out var resolvedValue, out var id, out _)) return resolvedValue;

            var value = (MoonSharpTableSerial)existingValue ?? new MoonSharpTableSerial();
            ReferenceResolverUtility.AddReference(id, value, serializer);
            while (true)
            {
                if (reader.TokenType == JsonToken.EndObject) break;

                var propertyName = (string)reader.Value;

                reader.Read();

                var memberValue = (double)serializer.Deserialize(reader, typeof(double));
                value.Load(propertyName, DynValue.NewNumber(memberValue));

                reader.Read();
            }
            return value;
        }
    }
}
