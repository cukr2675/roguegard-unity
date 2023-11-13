using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;

namespace ObjectFormer
{
    public class DependencyModuleTable<T> : IDependencyModuleTable<T>
        where T : IDependencyModule
    {
        private readonly Dictionary<string, List<T>> table;

        public DependencyModuleTable()
        {
            table = new Dictionary<string, List<T>>();
        }

        public void Add(T module)
        {
            if (!table.TryGetValue(module.Name, out var list))
            {
                list = new List<T>();
                table.Add(module.Name, list);
            }

            if (list.Any(x => x.Version == module.Version))
            {
                throw new Exception($"{module.Name} のバージョン {module.Version} はすでに追加されています。");
            }

            // Version の昇順で追加
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (module.Version > list[i].Version)
                {
                    list.Insert(i + 1, module);
                    return;
                }
            }
            list.Insert(0, module);
        }

        public bool TryGetModule(string name, string versionText, out T module)
        {
            if (!table.TryGetValue(name, out var list))
            {
                module = default;
                return false;
            }

            Version newerVersion, olderVersion;
            if (versionText.StartsWith('^'))
            {
                versionText = versionText.Substring(1);
                olderVersion = Version.Parse(versionText);
                newerVersion = new Version(olderVersion.Major, int.MaxValue);
            }
            else if (versionText.StartsWith('~'))
            {
                versionText = versionText.Substring(1);
                olderVersion = Version.Parse(versionText);
                newerVersion = new Version(olderVersion.Major, olderVersion.Minor, int.MaxValue);
            }
            else
            {
                olderVersion = Version.Parse(versionText);
                newerVersion = olderVersion;
            }

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (olderVersion <= list[i].Version && list[i].Version <= newerVersion)
                {
                    module = list[i];
                    return true;
                }
            }
            module = default;
            return false;
        }
    }
}
