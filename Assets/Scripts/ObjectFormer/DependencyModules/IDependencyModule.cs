using System;
using System.Collections;
using System.Collections.Generic;

namespace ObjectFormer
{
    public interface IDependencyModule
    {
        string Name { get; }
        Version Version { get; }
    }
}
