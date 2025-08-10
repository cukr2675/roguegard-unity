using System;

namespace Objforming
{
    // 実装忘れがないように Inherited = false にする。
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
    public class IgnoreRequireRelationalComponentAttribute : Attribute
    {
    }
}
