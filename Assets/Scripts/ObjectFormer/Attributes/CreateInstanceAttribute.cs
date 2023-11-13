using System;
using System.Collections;
using System.Collections.Generic;

namespace ObjectFormer
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public sealed class CreateInstanceAttribute : Attribute
    {
    }
}
