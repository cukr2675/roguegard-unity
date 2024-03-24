using System;
using System.Collections;
using System.Collections.Generic;

namespace Objforming
{
    public interface IDependencyModule
    {
        string Name { get; }
        Version Version { get; }
    }
}
