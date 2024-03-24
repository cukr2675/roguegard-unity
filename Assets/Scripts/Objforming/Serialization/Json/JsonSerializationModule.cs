using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using Newtonsoft.Json;

namespace Objforming.Serialization.Json
{
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
            string name, Version version, IEnumerable<RelationalJsonConverter> converters, IReadOnlyDictionary<string, string> dependencies = null)
        {
            Name = name;
            _version = version.ToString();

            if (dependencies != null)
            {
                this.dependencies = new Dictionary<string, string>(dependencies);
            }
            else
            {
                this.dependencies = new Dictionary<string, string>();
            }

            _converters = converters.ToArray();
        }

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
