using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Objforming.Serialization.Json
{
    /// <summary>
    /// <see cref="RelationalJsonConverter"/> とその依存関係定義からなるモジュール。
    /// このモジュールを組み合わせて <see cref="JsonSerializationConfig"/> を作成する。
    /// </summary>
    public class JsonSerializationModule : IDependencyModule
    {
        [JsonProperty("name")] public string Name { get; }

        [JsonProperty("version")] private readonly string _version;

        private Version _classVersion;
        [JsonIgnore] public Version Version => _classVersion ??= Version.Parse(_version);

        [JsonProperty("dependencies")] private readonly Dictionary<string, string> dependencies;

        [JsonProperty("converters")] private readonly RelationalJsonConverter[] _converters;

        private JsonSerializationModule() { }

        public JsonSerializationModule(
            string name, Version version, IEnumerable<RelationalJsonConverter> converters, IEnumerable<KeyValuePair<string, string>> dependencies = null)
        {
            Name = name;
            _version = version.ToString();
            _converters = converters.ToArray();

            if (dependencies != null) { this.dependencies = new Dictionary<string, string>(dependencies); }
            else { this.dependencies = new Dictionary<string, string>(); }
        }

        /// <summary>
        /// 指定のモジュールテーブルで依存関係を解決し、コンバータのリストを取得する
        /// </summary>
        public RelationalJsonConverter[] GetAllConverters(IDependencyModuleTable<JsonSerializationModule> moduleTable)
        {
            var allConverters = new List<RelationalJsonConverter>();
            foreach (var pair in dependencies)
            {
                if (!moduleTable.TryGetModule(pair.Key, pair.Value, out var module))
                {
                    ObjformingLogger.LogWarning($"モジュール {pair.Key} : {pair.Value} が見つかりませんでした。");
                    continue;
                }

                var moduleAllConverters = module.GetAllConverters(moduleTable);
                allConverters.AddComponents(moduleAllConverters);
            }
            allConverters.AddComponents(_converters);
            return allConverters.ToArray();
        }
    }
}
