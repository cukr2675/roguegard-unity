using System;
using System.Collections;
using System.Collections.Generic;

namespace ObjectFormer
{
    public interface IDependencyModuleTable<T>
        where T : IDependencyModule
    {
        bool TryGetModule(string name, string version, out T module);
    }
}
