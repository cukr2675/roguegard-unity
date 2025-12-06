using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Objforming.Serialization.TextJson
{
    public class JsonSerializationConfig
    {
        private readonly ObjectJsonConverter objConverter;
        private readonly ReferenceJsonConverter referenceConverter;
        private readonly List<RelationalJsonConverter> converters;

        public JsonSerializationConfig(IEnumerable<JsonSerializationModule> modules, IDependencyModuleTable<JsonSerializationModule> moduleTable)
        {
            objConverter = new ObjectJsonConverter { parent = this };
            referenceConverter = new ReferenceJsonConverter();
            converters = new List<RelationalJsonConverter>();

            foreach (var module in modules)
            {
                AddModule(module, moduleTable);
            }
        }

        private void AddModule(JsonSerializationModule module, IDependencyModuleTable<JsonSerializationModule> moduleTable)
        {
            var allConverters = module.GetAllConverters(moduleTable);
            foreach (var converter in allConverters)
            {
                converters.Add(converter);
                referenceConverter.RegisterReferableInstances(converter.References);
            }
        }

        public void Serialize<T>(Stream stream, T instance)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = new ObjformingReferenceHandler(true)
            };
            foreach (var converter in converters)
            {
                options.Converters.Add(converter);
            }
            options.Converters.Add(referenceConverter);
            options.Converters.Add(objConverter); // インターフェースの処理に必要

            JsonSerializer.Serialize(stream, instance, options);
        }

        public T Deserialize<T>(Stream stream)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = new ObjformingReferenceHandler(true)
            };
            foreach (var converter in converters)
            {
                options.Converters.Add(converter);
            }
            options.Converters.Add(objConverter);
            referenceConverter.SetReferences(options);

            return JsonSerializer.Deserialize<T>(stream, options);
        }

        /// <summary>
        /// 一部を除いたすべての型をコンバート対象とするため、 <see cref="JsonSerializer.Converters"/> の最後に設定する必要がある。
        /// このコンバーターには WriteJson を実装しない。（WriteJson が呼び出された時点でデフォルトのシリアル化ができなくなるため）
        /// </summary>
        private class ObjectJsonConverter : JsonConverter<object>
        {
            public JsonSerializationConfig parent;

            public override bool CanConvert(Type typeToConvert)
            {
                if (typeToConvert.IsPrimitive) return false;
                if (typeToConvert == typeof(string)) return false;
                if (typeToConvert == typeof(decimal)) return false;
                if (typeToConvert == typeof(Type)) return false;
                if (typeToConvert.IsEnum) return false;
                if (typeToConvert.IsArray) return false;
                if (typeToConvert.IsGenericType)
                {
                    var objectTypeDefinition = typeToConvert.GetGenericTypeDefinition();
                    if (objectTypeDefinition == typeof(List<>)) return false;
                    if (objectTypeDefinition == typeof(Dictionary<,>)) return false;
                }

                // 上記を除いた型をコンバートの対象とする
                return true;
            }

            public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
            {
                //if (value == null)
                //{
                //    writer.WriteNullValue();
                //    return;
                //}

                var type = value.GetType();
                foreach (var converter in parent.converters)
                {
                    if (converter.CanConvert(type))
                    {
                        converter.Write(writer, value, options);
                        return;
                    }
                }
                if (parent.referenceConverter.CanConvert(type))
                {
                    parent.referenceConverter.Write(writer, value, options);
                    return;
                }
                throw new InvalidOperationException($"{type} ({value}) に対応するコンバーターが見つかりません。");
            }

            public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                //if (reader.TokenType == JsonTokenType.Null) return null;

                var tempReader = reader;
                if (ReferenceResolverUtility.ReadTryResolveReference(ref reader, options, out var resolvedValue, out _, out var typeText)) return resolvedValue;

                // json の $type から型名を取得する
                // $type が設定されていなければ objectType をそのまま使用する
                var type = ReferenceResolverUtility.GetType(typeText, typeToConvert);

                // $type をもとにデシリアライズする
                foreach (var converter in parent.converters)
                {
                    if (converter.CanConvert(type))
                    {
                        reader = tempReader;
                        return converter.Read(ref reader, type, options);
                    }
                }

                // 対応するコンバーターがなければ例外を投げる
                throw new InvalidOperationException($"{typeToConvert} に対応するコンバーターが見つかりません。");
            }
        }
    }
}
