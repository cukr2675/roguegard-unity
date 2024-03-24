using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace Objforming.Serialization.Json
{
    public class JsonSerializationConfig
    {
        private readonly JsonSerializer serializer;
        private readonly ObjectJsonConverter objConverter;
        private readonly ReferenceJsonConverter referenceConverter;

        public JsonSerializationConfig(IEnumerable<JsonSerializationModule> modules, IDependencyModuleTable<JsonSerializationModule> moduleTable)
        {
            serializer = JsonSerializer.Create();
            serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            serializer.TypeNameHandling = TypeNameHandling.Auto;
            serializer.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
            //serializer.Formatting = Formatting.Indented;

            objConverter = new ObjectJsonConverter();
            objConverter.parent = this;
            referenceConverter = new ReferenceJsonConverter();

            foreach (var module in modules)
            {
                var converters = module.GetAllConverters(moduleTable);
                foreach (var converter in converters)
                {
                    serializer.Converters.Add(converter);
                    referenceConverter.Add(converter.References);
                }
            }
            serializer.Converters.Add(referenceConverter);
            serializer.Converters.Add(objConverter);
        }

        public void Serialize<T>(JsonWriter writer, T instance)
        {
            serializer.ReferenceResolver = new ObjformingReferenceResolver(true);

            serializer.Serialize(writer, instance, typeof(T));
        }

        public void Serialize<T>(Stream stream, T instance)
        {
            var streamWriter = new StreamWriter(stream);
            var writer = new JsonTextWriter(streamWriter);
            Serialize(writer, instance);
            writer.Flush();
        }

        public T Deserialize<T>(JsonReader reader)
        {
            serializer.ReferenceResolver = new ObjformingReferenceResolver(true);
            referenceConverter.SetReferences(serializer);

            return serializer.Deserialize<T>(reader);
        }

        public T Deserialize<T>(Stream stream)
        {
            var streamReader = new StreamReader(stream);
            var reader = new JsonTextReader(streamReader);
            return Deserialize<T>(reader);
        }

        /// <summary>
        /// 一部を除いたすべての型をコンバート対象とするため、 <see cref="JsonSerializer.Converters"/> の最後に設定する必要がある。
        /// このコンバーターには WriteJson を実装しない。（WriteJson が呼び出された時点でデフォルトのシリアル化ができなくなるため）
        /// </summary>
        private class ObjectJsonConverter : CustomCreationConverter<object>
        {
            public JsonSerializationConfig parent;

            public override bool CanConvert(Type objectType)
            {
                if (objectType.IsPrimitive) return false;
                if (objectType == typeof(string)) return false;
                if (objectType == typeof(decimal)) return false;
                if (objectType == typeof(Type)) return false;
                if (objectType.IsArray) return false;
                if (objectType.IsGenericType)
                {
                    var objectTypeDefinition = objectType.GetGenericTypeDefinition();
                    if (objectTypeDefinition == typeof(List<>)) return false;
                    if (objectTypeDefinition == typeof(Dictionary<,>)) return false;
                }
                return true;
                //return base.CanConvert(objectType);
            }

            public override object Create(Type objectType)
            {
                return Activator.CreateInstance(objectType, true);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) return null;

                var jObj = JObject.Load(reader);
                if (JsonConverterUtility.TryResolveReference(jObj["$ref"]?.ToString(), serializer, out var referencedValue)) return referencedValue;

                using var jObjReader = jObj.CreateReader();
                jObjReader.Read();

                // json の $type から型名を取得する
                // $type が設定されていなければ objectType をそのまま使用する
                var type = JsonConverterUtility.Text2Type(jObj["$type"]?.ToString(), objectType, serializer);

                foreach (var converter in serializer.Converters)
                {
                    if (converter != this && converter.CanRead && converter.CanConvert(type))
                    {
                        return converter.ReadJson(jObjReader, type, existingValue, serializer);
                    }
                }

                if (type.IsDefined(typeof(FormableAttribute))) throw new JsonException(
                    $"{type} は {nameof(FormableAttribute)} が設定されているため、自動 Json 化はサポートされません。");
                if (type.IsDefined(typeof(ReferableAttribute))) throw new JsonException(
                    $"{type} は {nameof(ReferableAttribute)} が設定されているため、自動 Json 化はサポートされません。");
                if (type.IsDefined(typeof(IgnoreRequireRelationalComponentAttribute))) throw new JsonException(
                    $"{type} は {nameof(IgnoreRequireRelationalComponentAttribute)} が設定されているため、自動 Json 化はサポートされません。");

                // 対応するコンバーターがなければデフォルトに任せる
                // （existingValue != null のときは ReadJson は実行されない）
                ObjformingLogger.Log($"{type} はデフォルトの Json 化を使用します。");
                var contract = serializer.ContractResolver.ResolveContract(type);
                var value = contract.DefaultCreator?.Invoke() ?? throw new JsonException(
                    $"{type} をインスタンスにできません。 Json が不正であるか、 {type} 向けコンバーターが不足している可能性があります。");
                serializer.Populate(jObjReader, value);
                return value;
                //return base.ReadJson(jObjReader, type, existingValue, serializer);
            }
        }
    }
}
