using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Newtonsoft.Json;
using MoonSharp.Interpreter;
using Objforming.Serialization.Json;

namespace Roguegard.Scripting.MoonSharp.Objforming.Serialization.Json
{
    public class MoonSharpTableSerialJsonConverter : RelationalJsonConverter
    {
        public override Type InstanceType => typeof(MoonSharpTableSerial);

        public override IReadOnlyList<Type> FieldTypes => _fieldTypes;
        private static readonly IReadOnlyList<Type> _fieldTypes = new Type[1] { typeof(Dictionary<string, object>) };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            if (JsonConverterUtility.WriteReferenceOrIDAndType(writer, value, serializer))
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

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            if (JsonConverterUtility.ReadTryResolveReference(reader, serializer, out var referencedValue, out var id, out _)) return referencedValue;

            var value = (MoonSharpTableSerial)existingValue ?? new MoonSharpTableSerial();
            JsonConverterUtility.AddReference(id, value, serializer);
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
