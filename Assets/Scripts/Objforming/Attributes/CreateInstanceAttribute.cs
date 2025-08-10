using System;

namespace Objforming
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public sealed class CreateInstanceAttribute : Attribute
    {
    }
}
