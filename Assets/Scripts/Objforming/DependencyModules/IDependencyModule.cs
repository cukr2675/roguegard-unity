using System;

namespace Objforming
{
    public interface IDependencyModule
    {
        string Name { get; }
        Version Version { get; }
    }
}
