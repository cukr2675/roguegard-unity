using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Objforming.Unity.RuntimeInspector
{
    public class InspectorModule : IDependencyModule
    {
        public string Name { get; }

        public System.Version Version { get; }

        private readonly Dictionary<string, string> dependencies;

        private readonly RelationalForm[] _forms;

        public InspectorModule(
            string name, System.Version version, IEnumerable<RelationalForm> forms, IReadOnlyDictionary<string, string> dependencies = null)
        {
            Name = name;
            Version = version;

            if (dependencies != null)
            {
                this.dependencies = new Dictionary<string, string>(dependencies);
            }
            else
            {
                this.dependencies = new Dictionary<string, string>();
            }

            _forms = forms.ToArray();
        }

        public RelationalForm[] GetAllForms(IDependencyModuleTable<InspectorModule> moduleTable)
        {
            var allForms = new List<RelationalForm>();
            foreach (var pair in dependencies)
            {
                if (!moduleTable.TryGetModule(pair.Key, pair.Value, out var module)) throw new System.Exception(
                    $"ÉÇÉWÉÖÅ[Éã {pair.Key} : {pair.Value} Ç™å©Ç¬Ç©ÇËÇ‹ÇπÇÒÇ≈ÇµÇΩÅB");

                var moduleAllConverters = module.GetAllForms(moduleTable);
                allForms.AddComponents(moduleAllConverters);
            }
            allForms.AddComponents(_forms);
            return allForms.ToArray();
        }
    }
}
