using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

namespace ObjectFormer.Serialization.TextJson
{
    [Formable]
    public class JsonSerializationModule : IDependencyModule
    {
        public string Name { get; }

        private string _version;

        [System.NonSerialized] private Version _classVersion;
        public Version Version => _classVersion ??= Version.Parse(_version);

        private readonly Dictionary<string, string> dependencies;

        private readonly RelationalJsonConverter[] _converters;

        private JsonSerializationModule() { }

        public JsonSerializationModule(
            string name, Version version, IEnumerable<RelationalJsonConverter> converters, IReadOnlyDictionary<string, string> dependencies = null)
        {
            Name = name;
            _version = version.ToString();

            this.dependencies = new Dictionary<string, string>();
            if (dependencies != null)
            {
                foreach (var pair in dependencies)
                {
                    this.dependencies.Add(pair.Key, pair.Value);
                }
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
                    ObjectFormerLogger.LogWarning($"モジュール {pair.Key} : {pair.Value} が見つかりませんでした。");
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
