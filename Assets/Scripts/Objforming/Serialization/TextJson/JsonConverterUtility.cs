using System;
using System.Collections;
using System.Collections.Generic;

using System.Reflection;
using System.Text.Json;

namespace Objforming.Serialization.TextJson
{
    public static class JsonConverterUtility
    {
        /// <summary>
        /// $ref または $id と $type を書き込む。
        /// 続けて <paramref name="value"/> の内容を書き込む必要がある場合は true を返す。
        /// </summary>
        public static bool WriteReferenceOrIDAndType(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            var type = value.GetType();
            if (!type.IsValueType)
            {
                // 値型でなければ $id か $ref を書き込む
                var referenceResolver = options.ReferenceHandler.CreateResolver();
                var id = referenceResolver.GetReference(value, out var alreadyExists);
                if (alreadyExists)
                {
                    // すでに参照が存在している場合、それを記録する
                    writer.WriteString("$ref", id);
                    return false;
                }
                else
                {
                    writer.WriteString("$id", id);
                }
            }
            WriteType(writer, type);
            return true;
        }

        private static void WriteType(Utf8JsonWriter writer, Type type)
        {
            var assemblyName = type.Assembly.GetName().Name;
            var typeName = type.ToString();
            writer.WriteString("$type", $"{typeName}, {assemblyName}");
        }

        public static bool ReadTryResolveReference(
            ref Utf8JsonReader reader, JsonSerializerOptions options, out object referencedValue, out string id, out string typeText)
        {
            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException($"{reader.TokenType} is not {JsonTokenType.StartObject}");

            referencedValue = null;
            id = null;
            typeText = null;
            while (true)
            {
                reader.Read();
                if (reader.TokenType == JsonTokenType.EndObject) break;

                var propertyName = reader.GetString();
                if (propertyName == "$ref")
                {
                    reader.Read();
                    var reference = reader.GetString();
                    var referenceResolver = options.ReferenceHandler.CreateResolver();
                    referencedValue = referenceResolver.ResolveReference(reference);
                    if (referencedValue == null) { ObjformingLogger.LogWarning($"$ref: {reference} は null です。"); }
                }
                else if (propertyName == "$id")
                {
                    reader.Read();
                    id = reader.GetString();
                }
                else if (propertyName == "$type")
                {
                    reader.Read();
                    typeText = reader.GetString();
                }
                else
                {
                    break;
                }
            }
            return referencedValue != null;
        }

        public static bool TryResolveReference(string reference, JsonSerializerOptions options, out object referencedValue)
        {
            if (reference != null)
            {
                var referenceResolver = options.ReferenceHandler.CreateResolver();
                referencedValue = referenceResolver.ResolveReference(reference);
                if (referencedValue == null) { ObjformingLogger.LogWarning($"$ref: {reference} は null です。"); }
                return true;
            }
            else
            {
                referencedValue = null;
                return false;
            }
        }

        public static void AddReference(string id, object value, JsonSerializerOptions options)
        {
            if (id != null)
            {
                var referenceResolver = options.ReferenceHandler.CreateResolver();
                referenceResolver.AddReference(id, value);
            }
        }

        public static Type Text2Type(string typeText, Type objectType)
        {
            if (typeText != null)
            {
                Split(typeText, out var typeName, out var assemblyName);
                var assembly = Assembly.Load(assemblyName);
                var type = assembly.GetType(typeName);
                return type;
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
