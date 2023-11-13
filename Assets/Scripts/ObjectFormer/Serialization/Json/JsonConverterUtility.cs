using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace ObjectFormer.Serialization.Json
{
    public static class JsonConverterUtility
    {
        /// <summary>
        /// $ref または $id と $type を書き込む。
        /// 続けて <paramref name="value"/> の内容を書き込む必要がある場合は true を返す。
        /// </summary>
        public static bool WriteReferenceOrIDAndType(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (serializer.ReferenceResolver.IsReferenced(serializer, value))
            {
                // すでに参照が存在している場合、それを記録する
                var id = serializer.ReferenceResolver.GetReference(serializer, value);
                writer.WritePropertyName("$ref");
                writer.WriteValue(id);
                return false;
            }
            else
            {
                var type = value.GetType();
                if (serializer.PreserveReferencesHandling != PreserveReferencesHandling.None &&
                    serializer.PreserveReferencesHandling != PreserveReferencesHandling.Arrays &&
                    !type.IsValueType)
                {
                    // 値型でなければ $id を書き込む
                    var id = serializer.ReferenceResolver.GetReference(serializer, value);
                    writer.WritePropertyName("$id");
                    writer.WriteValue(id);
                }
                if (serializer.TypeNameHandling != TypeNameHandling.None &&
                    serializer.TypeNameHandling != TypeNameHandling.Arrays)
                {
                    WriteType(writer, type, serializer);
                }
                return true;
            }
        }

        private static void WriteType(JsonWriter writer, Type type, JsonSerializer serializer)
        {
            writer.WritePropertyName("$type");
            //serializer.Serialize(writer, type, typeof(Type)); // これは TypeNameAssemblyFormatHandling.Simple が無効になる

            serializer.SerializationBinder.BindToName(type, out var assemblyName, out _);
            if (serializer.TypeNameAssemblyFormatHandling == TypeNameAssemblyFormatHandling.Simple)
            {
                assemblyName = assemblyName.Substring(0, assemblyName.IndexOf(','));
            }
            var typeName = type.ToString();
            writer.WriteValue($"{typeName}, {assemblyName}");
        }

        public static bool ReadTryResolveReference(JsonReader reader, JsonSerializer serializer, out object referencedValue, out string id, out string typeText)
        {
            if (reader.TokenType != JsonToken.StartObject) throw new JsonException($"{reader.TokenType} is not {JsonToken.StartObject}");

            referencedValue = null;
            id = null;
            typeText = null;
            while (true)
            {
                reader.Read();
                if (reader.TokenType == JsonToken.EndObject) break;

                var propertyName = (string)reader.Value;
                if (propertyName == "$ref")
                {
                    reader.Read();
                    var reference = (string)reader.Value;
                    referencedValue = serializer.ReferenceResolver.ResolveReference(serializer, reference);
                    if (referencedValue == null) { ObjectFormerLogger.LogWarning($"$ref: {reference} は null です。"); }
                }
                else if (propertyName == "$id")
                {
                    reader.Read();
                    id = (string)reader.Value;
                }
                else if (propertyName == "$type")
                {
                    reader.Read();
                    typeText = (string)reader.Value;
                }
                else
                {
                    break;
                }
            }
            return referencedValue != null;
        }

        public static bool TryResolveReference(string reference, JsonSerializer serializer, out object referencedValue)
        {
            if (reference != null)
            {
                referencedValue = serializer.ReferenceResolver.ResolveReference(serializer, reference);
                if (referencedValue == null) { ObjectFormerLogger.LogWarning($"$ref: {reference} は null です。"); }
                return true;
            }
            else
            {
                referencedValue = null;
                return false;
            }
        }

        public static void AddReference(string id, object value, JsonSerializer serializer)
        {
            if (id != null) { serializer.ReferenceResolver.AddReference(serializer, id, value); }
        }

        public static Type Text2Type(string typeText, Type objectType, JsonSerializer serializer)
        {
            if (typeText != null)
            {
                Split(typeText, out var typeName, out var assemblyName);
                return serializer.SerializationBinder.BindToType(assemblyName, typeName);
            }
            else
            {
                return objectType;
            }
        }

        private static void Split(string typeText, out string typeName, out string assemblyName)
        {
            var typeAssemblySeparatorIndex = GetTypeAssemblySeparatorIndex(typeText);

            typeName = typeText.Substring(0, typeAssemblySeparatorIndex).Trim();
            assemblyName = typeText.Substring(typeAssemblySeparatorIndex + 1).Trim();

            int GetTypeAssemblySeparatorIndex(string text)
            {
                var depth = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    var item = text[i];
                    if (item == '[') { depth++; }
                    else if (item == ']') { depth--; }
                    else if (item == ',' && depth == 0) return i;
                }
                throw new Exception();
            }
        }
    }
}
