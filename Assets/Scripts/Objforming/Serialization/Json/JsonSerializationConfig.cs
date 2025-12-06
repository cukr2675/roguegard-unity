using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Objforming.Serialization.Json
{
    public class JsonSerializationConfig
    {
        private readonly JsonSerializer serializer;
        private readonly ReferenceJsonConverter referenceJsonConverter;

        public JsonSerializationConfig(IEnumerable<JsonSerializationModule> modules, IDependencyModuleTable<JsonSerializationModule> moduleTable)
        {
            serializer = JsonSerializer.Create();
            serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            serializer.TypeNameHandling = TypeNameHandling.None; // JsonConverter が介入すると $type が効かないようなので必要ない（無効化する）
            serializer.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;

            referenceJsonConverter = new ReferenceJsonConverter();
            foreach (var module in modules)
            {
                var converters = module.GetAllConverters(moduleTable);
                foreach (var converter in converters)
                {
                    serializer.Converters.Add(converter);
                    referenceJsonConverter.RegisterReferableInstances(converter.References);
                }
            }

            serializer.Converters.Add(referenceJsonConverter);
            serializer.Converters.Add(new ObjectJsonConverter { parent = this });
        }

        public void Serialize<T>(JsonWriter writer, T value)
        {
            serializer.ReferenceResolver = new ObjformingReferenceResolver(true);

            serializer.Serialize(writer, value, typeof(T));
        }

        public void Serialize<T>(Stream stream, T value)
        {
            var streamWriter = new StreamWriter(stream);
            var writer = new JsonTextWriter(streamWriter);
            Serialize(writer, value);
            writer.Flush(); // JsonTextWriter の完了処理
        }

        public T Deserialize<T>(JsonReader reader)
        {
            serializer.ReferenceResolver = new ObjformingReferenceResolver(true);
            referenceJsonConverter.SetReferences(serializer);

            return serializer.Deserialize<T>(reader);
        }

        public T Deserialize<T>(Stream stream)
        {
            var streamReader = new StreamReader(stream);
            var reader = new JsonTextReader(streamReader);
            return Deserialize<T>(reader);
        }

        /// <summary>
        /// ほぼすべての型をコンバート対象とするため、 <see cref="JsonSerializer.Converters"/> の最後に設定する必要がある。
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
                if (objectType.IsEnum) return false;
                if (objectType.IsArray) return false;
                if (objectType.IsGenericType)
                {
                    var objectTypeDefinition = objectType.GetGenericTypeDefinition();
                    if (objectTypeDefinition == typeof(List<>)) return false;
                    if (objectTypeDefinition == typeof(Dictionary<,>)) return false;
                }

                // 上記を除いた型をコンバートの対象とする
                return true;
            }

            public override object Create(Type objectType)
            {
                return Activator.CreateInstance(objectType, true);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                // "null" は null を返す
                if (reader.TokenType == JsonToken.Null) return null;

                // jObj に $ref が設定されている場合は解決を試行し、解決できればその値を返す
                var jObj = JObject.Load(reader);
                if (ReferenceResolverUtility.TryResolveReference(jObj, serializer, out var resolvedValue)) return resolvedValue;

                using var jObjReader = jObj.CreateReader();
                jObjReader.Read();

                // json の $type から型名を取得する
                // $type が設定されていなければ objectType をそのまま使用する
                var type = ReferenceResolverUtility.GetType(jObj, objectType, serializer);

                // $type をもとにデシリアライズする
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
            }
        }
    }
}
