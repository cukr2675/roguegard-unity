using System;
using System.Collections;
using System.Collections.Generic;

namespace Objforming
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public sealed class CreateInstanceAttribute : Attribute
    {
    }
}
